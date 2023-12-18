using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit.ESP
{
    internal class ESPPlayerInfo
    {
        public string shortName { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string href { get; set; } = string.Empty;
        public string uid { get; set; } = string.Empty;
        public string guid { get; set; } = string.Empty;
        public string id { get; set; } = string.Empty;
        public string height { get; set; } = string.Empty;
        public string weight { get; set; } = string.Empty;
        public int age { get; set; }
        public string position { get; set; } = string.Empty;
        public string hand { get; set; } = string.Empty;
        public string jersey { get; set; } = string.Empty;
        public string birthPlace { get; set; } = string.Empty;
        public string birthDate { get; set; } = string.Empty;
        public string headshot { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public int experience { get; set; }
    }

}
