using StaplePuck.Core.Fantasy;
using StaplePuck.Core.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.Request
{
    public class Player
    {
        public string FullName { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Number { get; set; }
    }
}
