using System;
using System.Collections.Generic;
using System.Text;

namespace NHLPlayoffSeasonInit.Data
{
    public class Standings
    {
        public class Result
        {
            public string copyright { get; set; }
            public Record[] records { get; set; }
        }

        public class Record
        {
            public string standingsType { get; set; }
            public League league { get; set; }
            public Teamrecord[] teamRecords { get; set; }
        }

        public class League
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }

        public class Teamrecord
        {
            public Team team { get; set; }
            public string leagueRank { get; set; }
            public string wildCardRank { get; set; }
            public string clinchIndicator { get; set; }
            public DateTime lastUpdated { get; set; }
        }

        public class Team
        {
            public int id { get; set; }
            public string name { get; set; }
            public string link { get; set; }
        }
    }
}
