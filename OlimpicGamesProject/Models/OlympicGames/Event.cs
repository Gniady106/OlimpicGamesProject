using System;
using System.Collections.Generic;

namespace OlimpicGamesProject.Models.OlympicGames;

public partial class Event
{
    public int Id { get; set; }

    public int? SportId { get; set; }

    public string? EventName { get; set; }

    public virtual Sport? Sport { get; set; }
}
