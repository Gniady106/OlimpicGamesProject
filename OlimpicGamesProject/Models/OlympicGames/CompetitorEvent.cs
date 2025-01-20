using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlimpicGamesProject.Models.OlympicGames;

public partial class CompetitorEvent
{
    
    [ForeignKey("Event")]
    public int? EventId { get; set; }

    [ForeignKey("Competitor")]
    public int? CompetitorId { get; set; }

    [ForeignKey("Medal")]
    public int? MedalId { get; set; }

    public virtual GamesCompetitor? Competitor { get; set; }

    public virtual Event? Event { get; set; }

    public virtual Medal? Medal { get; set; }
}
