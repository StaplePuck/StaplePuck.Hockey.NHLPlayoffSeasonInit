using System;
using System.Collections.Generic;
using System.Text;

namespace NHLPlayoffSeasonInit
{
    public class SeasonRequest
    {
        public string SeasonId { get; set; } = string.Empty;
        public string GameDate { get; set; } = string.Empty;
        public int StartRound { get; set; }
        public string SeasonName { get; set; } = string.Empty;
    }
}
