using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.NHL
{
    public class Franchises
    {
        public Franchise[] data { get; set; } = new Franchise[0];

        public class Franchise
        {
            public int id { get; set; }
            public string fullName { get; set; } = string.Empty;
            public string teamCommonName { get; set; } = string.Empty;
            public string teamPlaceName { get; set; } = string.Empty;
        }
    }
}
