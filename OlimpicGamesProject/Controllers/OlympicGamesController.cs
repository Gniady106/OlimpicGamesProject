using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpicGamesProject.Models.OlympicGames;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OlimpicGamesProject.Models;

namespace OlimpicGamesProject.Controllers;

public class OlympicGamesController : Controller
{

    private readonly OlympicGamesDbContext _context;

    public OlympicGamesController(OlympicGamesDbContext context)
    {
        _context = context;
    }




    public async Task<IActionResult> ListOfAthletes(int page = 1, int size = 20)
    {
        var totalItems = await _context.People.CountAsync();


        var athletes = await (from person in _context.People
                join gamesCompetitor in _context.GamesCompetitors on person.Id equals gamesCompetitor.PersonId
                join competitorEvent in _context.CompetitorEvents on gamesCompetitor.Id equals competitorEvent
                    .CompetitorId into eventsGroup
                from eventGroup in eventsGroup.DefaultIfEmpty()
                join medal in _context.Medals on eventGroup.MedalId equals medal.Id into medalsGroup
                from medalGroup in medalsGroup.DefaultIfEmpty()
                group new { person, medalGroup } by new
                {
                    person.Id,
                    person.FullName,
                    person.Gender,
                    person.Height,
                    person.Weight
                }
                into g
                select new AthleteViewModel
                {
                    Id = g.Key.Id,
                    FullName = g.Key.FullName,
                    Gender = g.Key.Gender,
                    Height = g.Key.Height,
                    Weight = g.Key.Weight,
                    GoldMedals = g.Count(x => x.medalGroup != null && x.medalGroup.MedalName == "Gold"),
                    SilverMedals = g.Count(x => x.medalGroup != null && x.medalGroup.MedalName == "Silver"),
                    BronzeMedals = g.Count(x => x.medalGroup != null && x.medalGroup.MedalName == "Bronze"),
                    Competitions = g.Count(x => x.medalGroup != null)
                })
            .OrderBy(m => m.FullName)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();


        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / size);
        ViewBag.Size = size;

        return View(athletes);



    }

    [HttpGet]
    public async Task<IActionResult> Competitors(int competitorId)
    {
        var competitors = await (
            from gamesCompetitor in _context.GamesCompetitors
            join pearson in _context.People on gamesCompetitor.PersonId equals pearson.Id
            join games in _context.Games on gamesCompetitor.GamesId equals games.Id
            join competitorEvent in _context.CompetitorEvents on gamesCompetitor.Id equals competitorEvent.CompetitorId
            join @event in _context.Events on competitorEvent.EventId equals @event.Id
            join sport in _context.Sports on @event.SportId equals sport.Id
            join medal in _context.Medals on competitorEvent.MedalId equals medal.Id into medalsGroup
            from medalGroup in medalsGroup.DefaultIfEmpty()
            where gamesCompetitor.PersonId == competitorId
            select new CompetitorsEventViewModel
            {
                AthleteId = pearson.Id,
                SportName = sport.SportName,
                EventName = @event.EventName,
                Olympics = games.GamesName,
                AthleteAge = gamesCompetitor.Age,
                Medal = medalGroup != null ? medalGroup.MedalName : "NA"
            }).ToListAsync(); 
       
        return View(competitors);
    }

    [Authorize]
    public IActionResult AddToEvent(int athleteId)
    {
        
        
        
        
        var model = new AddToEventViewModel
        {
            AthleteId = athleteId,
            Sports = _context.Sports.ToList(),
            Events = _context.Events.ToList(),
            Games = _context.Games.ToList(),
            Medals = _context.Medals.ToList()
            
            
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToEvent(AddToEventViewModel model)
    {
        if (model.Age < 10 || model.Age > 110)
        {
            var modeld = new AddToEventViewModel
            {
                AthleteId = model.AthleteId,
                Sports = _context.Sports.ToList(),
                Events = _context.Events.ToList(),
                Games = _context.Games.ToList(),
                Medals = _context.Medals.ToList()
            
            };
            return View(modeld);
        }
        
        var maxId = await _context.GamesCompetitors.MaxAsync(g => (int?)g.Id) ?? 0;

        var gamesCompetitor = new GamesCompetitor()
        {
            Id = maxId + 1,
            GamesId = model.GamesId,
            PersonId = model.AthleteId,
            Age = model.Age
        };
    
        
        _context.GamesCompetitors.Add(gamesCompetitor);
        await _context.SaveChangesAsync();

        
        var competitorEvent = new CompetitorEvent
        {
            CompetitorId = gamesCompetitor.Id,
            EventId = model.EventId,
            MedalId = model.MedalId
        };
        _context.CompetitorEvents.Add(competitorEvent);
        await _context.SaveChangesAsync();

        return RedirectToAction("Competitors", new { competitorId = model.AthleteId });
    }
}