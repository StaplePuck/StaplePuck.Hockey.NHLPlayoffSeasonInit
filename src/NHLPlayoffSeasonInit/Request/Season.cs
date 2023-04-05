using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.Request
{
    public class Season
    {
        public string ExternalId { get; set; }
        public string FullName { get; set; }
        public Sport Sport { get; set; }
        public bool IsPlayoffs { get; set; }
        public int StartRound { get; set; }
        public List<PlayerSeason> PlayerSeasons { get; set; } = new List<PlayerSeason>();
    }
}
