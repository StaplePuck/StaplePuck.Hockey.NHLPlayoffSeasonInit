using System;
using System.Collections.Generic;
using System.Text;

namespace NHLPlayoffSeasonInit.Data
{
    public class Tournament
    {
        public class Result
        {
            public string copyright { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public string season { get; set; }
            public int defaultRound { get; set; }
            public Round[] rounds { get; set; }
        }

        public class Round
        {
            public int number { get; set; }
            public int code { get; set; }
            public Format format { get; set; }
            public Series[] series { get; set; }
        }

        public class Format
        {
            public string name { get; set; }
            public string description { get; set; }
            public int numberOfGames { get; set; }
            public int numberOfWins { get; set; }
        }

        public class Series
        {
            public int seriesNumber { get; set; }
            public string seriesCode { get; set; }
            public Matchupteam[] matchupTeams { get; set; }
        }

        public class Matchupteam
        {
            public Team team { get; set; }
        }

        public class Team
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }
    }
}
