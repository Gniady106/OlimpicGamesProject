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
            join games in _context.Games on gamesCompetitor.GamesId equals games.Id
            join competitorEvent in _context.CompetitorEvents on gamesCompetitor.Id equals competitorEvent.CompetitorId
            join @event in _context.Events on competitorEvent.EventId equals @event.Id
            join sport in _context.Sports on @event.SportId equals sport.Id
            join medal in _context.Medals on competitorEvent.MedalId equals medal.Id into medalsGroup
            from medalGroup in medalsGroup.DefaultIfEmpty()
            where gamesCompetitor.PersonId == competitorId
            select new CompetitorsEventViewModel()
            {
                SportName = sport.SportName,
                EventName = @event.EventName,
                Olympics = games.GamesName,
                Season = games.Season,
                AthleteAge = gamesCompetitor.Age,
                Medal = medalGroup != null ? medalGroup.MedalName : "None"
            }).ToListAsync();

        return View(competitors);
    }

    [Authorize]
    public async Task<IActionResult> AddToEvent(int athleteId)
    {
        var sports = await _context.Sports.ToListAsync();
        var events = await _context.Events.ToListAsync();

        var model = new AddToEventViewModel()
        {
            AthleteId = athleteId,
            Sports = _context.Sports.ToList(),
            Events = _context.Events.ToList(),
            Games = _context.Games.ToList()
            
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddToEvent(AddToEventViewModel model)
    {
        var maxId = await _context.GamesCompetitors.MaxAsync(g => (int?)g.Id) ?? 0;

        var gamescompetitor = new GamesCompetitor()
        {
            Id = maxId + 1,
            GamesId = model.GamesId,
            PersonId = model.AthleteId ,
            Age = model.Age
        };
        _context.GamesCompetitors.Add(gamescompetitor);
        await _context.SaveChangesAsync();
        
        
        return RedirectToAction("AddToEvent", new { athleteId = model.AthleteId });
    }

        // Jeśli coś poszło nie tak, ponownie wyświetl formularz
        // model.Sports = await _context.Sports
        //     .Select(s => new SelectListItem
        //     {
        //         Value = s.Id.ToString(),
        //         Text = s.SportName
        //     })
        //     .ToListAsync();
        //
        // model.Events = await _context.Events
        //     .Select(e => new SelectListItem
        //     {
        //         Value = e.Id.ToString(),
        //         Text = e.EventName
        //     })
        //     .ToListAsync();
        //
        // model.Games = await _context.Games
        //     .Select(g => new SelectListItem
        //     {
        //         Value = g.Id.ToString(),
        //         Text = g.GamesName
        //     })
        //     .ToListAsync();
        //
        // return View(model);
    
}
// [HttpGet]
    // public async Task<IActionResult> AddToEvent(int athleteId)
    // {
    //     var viewModel = new CompetitorsEventViewModel()
    //     {
    //         AthleteId = athleteId,
    //         Sports = await _context.Sports
    //             .Select(s => new SelectListItem
    //             {
    //                 Value = s.Id.ToString(),
    //                 Text = s.SportName
    //             })
    //             .ToListAsync(),
    //
    //         Events = await _context.Events
    //             .Select(e => new SelectListItem
    //             {
    //                 Value = e.Id.ToString(),
    //                 Text = e.EventName
    //             })
    //             .ToListAsync(),
    //         Medals = await _context.Medals
    //             .Select(m => new SelectListItem()
    //             {
    //                 Value = m.Id.ToString(),
    //                 Text = m.MedalName
    //                 
    //             })
    //             .ToListAsync(),
    //
    //         Games = await _context.Games
    //             .Select(g => new SelectListItem
    //             {
    //                 Value = g.Id.ToString(),
    //                 Text = g.GamesName
    //             })
    //             .ToListAsync()
    //     };
    //
    //     return View(viewModel);
    // }

//     [HttpPost]
//     public async Task<IActionResult> AddToEvent(CompetitorsEventViewModel model)
//     {
//         
//         // Sprawdzenie, czy istnieje już wpis w GamesCompetitors
         // using (var connection = _context.Database.GetDbConnection())
         // {
         //     connection.Open();
         //
         //     using (var command = connection.CreateCommand())
         //     {
         //         command.CommandText = "SELECT IFNULL(MAX(id), 0) + 1 FROM games_competitor;";
         //         var result = command.ExecuteScalar();
         //
         //         model.Id = Convert.ToInt32(result);
         //     }
         // }
//
//         GamesCompetitor newcompetitor = new GamesCompetitor
//         {
//             Id = model.Id,
//             PersonId = model.AthleteId,
//             GamesId = model.GamesId,
//             Age = model.AthleteAge ?? 0 // Jeśli wiek jest null, ustaw domyślną wartość 0
//         };
//
//         _context.GamesCompetitors.Add(newcompetitor);
//         await _context.SaveChangesAsync(); // Zapisanie do bazy, aby uzyskać Id nowego rekordu
//
// // Tworzenie nowego wpisu w CompetitorEvents
//         CompetitorEvent newcompetitorEvent = new CompetitorEvent
//         {
//             EventId = model.EventId,
//             CompetitorId = newcompetitor.Id, // Powiązanie z nowym GamesCompetitor
//             MedalId = model.MedalId
//         };
//
//         _context.CompetitorEvents.Add(newcompetitorEvent);
//         await _context.SaveChangesAsync();
//             
//
//             // Przekierowanie do listy sportowców
//         
//         
//             return RedirectToAction("ListOfAthletes");
//     }
//
//
// }