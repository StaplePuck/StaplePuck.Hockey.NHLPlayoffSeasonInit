using System;
using System.Collections.Generic;
using System.Text;

namespace NHLPlayoffSeasonInit.Data
{
    public class RosterInfo
    {

        public class Result
        {
            public Roster[] roster { get; set; }
        }

        public class Roster
        {
            public Person person { get; set; }
            public string jerseyNumber { get; set; }
            public Position position { get; set; }
        }

        public class Person
        {
            public int id { get; set; }
            public string fullName { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string primaryNumber { get; set; }
            public bool active { get; set; }
            public bool alternateCaptain { get; set; }
            public bool captain { get; set; }
            public bool rookie { get; set; }
            public string rosterStatus { get; set; }
        }

        public class Position
        {
            public string code { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string abbreviation { get; set; }
        }
    }
}
