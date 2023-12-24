using StaplePuck.Core.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.StaplePuck
{
    public class PlayerSeason
    {
        public Player? Player { get; set; }
        public Team? Team { get; set; }
        public PositionType? PositionType { get; set; }
    }
}
