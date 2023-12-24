using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit
{
    public class AssetRequest
    {
        public int SeasonId { get; set; }
        public string GameDate { get; set; } = string.Empty;
        public bool OverwriteLogos { get; set; } = false;
        public bool OverwriteHeadshots { get; set; } = false;
    }
}
