using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.Request
{
    public class Team
    {
        public string FullName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public int ExternalId { get; set; }
        public string? ExternalId2 { get; set; }
    }
}
