using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OlimpicGamesProject.Models.OlympicGames;

namespace OlimpicGamesProject.Models;


public class AddToEventViewModel
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public int SportId { get; set; } // Nowe pole
    public int EventId { get; set; }
    public int GamesId { get; set; }
    public int Age { get; set; }

    public List<Sport> Sports { get; set; } // Lista dyscyplin
    public List<Event> Events { get; set; } // Lista wydarzeń
    public List<Game> Games { get; set; }  // Lista olimpiad
}