﻿using System;
using System.Collections.Generic;

namespace OlimpicGamesProject.Models.OlympicGames;

public partial class GamesCity
{
    public int? GamesId { get; set; }

    public int? CityId { get; set; }

    public virtual City? City { get; set; }

    public virtual Game? Games { get; set; }
}
