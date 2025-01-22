using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using OlimpicGamesProject.Models.OlympicGames;

namespace OlimpicGamesProject.Models;


public class AddToEventViewModel
{
    public int Id { get; set;}
    public int AthleteId { get; set; }
    public int SportId { get; set; } // Nowe pole
    public int EventId { get; set; }
    public int GamesId { get; set; }
    
    public int MedalId { get; set; }
    
    [Range(10, 110, ErrorMessage = "Age must be between 10 and 110.")]
    public int? Age { get; set; }

    public List<Sport> Sports { get; set; } 
    public List<Event> Events { get; set; } 
    public List<Game> Games { get; set; }  
    public List<Medal> Medals { get; set; }  
}
