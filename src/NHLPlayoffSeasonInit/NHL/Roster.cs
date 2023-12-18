using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.NHL
{
    public class Roster
{
        public Player[] forwards { get; set; } = new Player[0];
        public Player[] defensemen { get; set; } = new Player[0];
        public Player[] goalies { get; set; } = new Player[0];

        public class Player
        {
            public int id { get; set; }
            public string headshot { get; set; } = string.Empty;
            public Firstname firstName { get; set; } = new Firstname();
            public Lastname lastName { get; set; } = new Lastname();
            public int sweaterNumber { get; set; }
            public string positionCode { get; set; } = string.Empty;
            public string birthCountry { get; set; } = string.Empty;
        }

        public class Firstname
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }

        public class Lastname
        {
            [JsonPropertyName("default")]
            public string _default { get; set; } = string.Empty;
        }
    }
}
