using Microsoft.AspNetCore.Mvc.Rendering;

namespace OlimpicGamesProject.Models.OlympicGames;

public class CompetitorsEventViewModel
{
    
    public string SportName { get; set; }
    public string EventName { get; set; }
    public string Olympics { get; set; }
    public string Season { get; set; }
    public int? AthleteAge { get; set; }
    
    public string Medal { get; set; }
    
}