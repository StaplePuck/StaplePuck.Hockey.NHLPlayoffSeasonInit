using Stats = StaplePuck.Core.Stats;

namespace NHLPlayoffSeasonInit.StaplePuck
{
    public interface IStaplePuckProvider
    {
        Task CreateSeaseonAsync(Season season, CancellationToken cancellationToken);
        Task<Stats.Season?> GetSesaonPlayersAsync(int seasonId, CancellationToken cancellation);
    }
}