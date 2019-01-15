using System;
using System.Collections.Generic;
using System.Text;

namespace NHLPlayoffSeasonInit.Data
{
    public class TeamInfo
    {

        public class Result
        {
            public string copyright { get; set; }
            public Team[] teams { get; set; }
        }

        public class Team
        {
            public int id { get; set; }
            public string name { get; set; }
            public string abbreviation { get; set; }
            public string teamName { get; set; }
            public string locationName { get; set; }
            public string shortName { get; set; }
        }
    }
}
