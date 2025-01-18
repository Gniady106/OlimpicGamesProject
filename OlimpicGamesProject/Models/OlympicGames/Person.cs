using System;
using System.Collections.Generic;

namespace OlimpicGamesProject.Models.OlympicGames;

public partial class Person
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Gender { get; set; }

    public int? Height { get; set; }

    public int? Weight { get; set; }

    public virtual ICollection<GamesCompetitor> GamesCompetitors { get; set; } = new List<GamesCompetitor>();
}
