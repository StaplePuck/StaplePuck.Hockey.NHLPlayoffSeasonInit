using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.NHL
{
    public class Standings
    {
        public Standing[] standings { get; set; } = new Standing[0];

        public class Standing
        {
            public string clinchIndicator { get; set; } = string.Empty;
            public string conferenceAbbrev { get; set; } = string.Empty;
            public string conferenceName { get; set; } = string.Empty;
            public string divisionAbbrev { get; set; } = string.Empty;
            public string divisionName { get; set; } = string.Empty;
            public int gameTypeId { get; set; }
            public int gamesPlayed { get; set; }
            public int seasonId { get; set; }
            public Placename placeName { get; set; } = new Placename();
            public Teamname teamName { get; set; } = new Teamname();
            public Teamcommonname teamCommonName { get; set; } = new Teamcommonname();
            public Teamabbrev teamAbbrev { get; set; } = new Teamabbrev();
            public string teamLogo { get; set; } = string.Empty;
        }

        public class Placename
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }

        public class Teamname
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }

        public class Teamcommonname
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }

        public class Teamabbrev
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }
    }
}
