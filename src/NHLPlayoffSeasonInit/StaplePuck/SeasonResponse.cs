using Stats = StaplePuck.Core.Stats;

namespace NHLPlayoffSeasonInit.StaplePuck
{
    internal class SeasonResponse
    {
        public Stats.Season[] Seasons { get; set; } = Array.Empty<Stats.Season>();
    }
}
