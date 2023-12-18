using NHLPlayoffSeasonInit.Request;

namespace NHLPlayoffSeasonInit
{
    public interface IRosterProvider
    {
        Task<Stream?> GetPlayerHeadShotAsync(PlayerSeason player, CancellationToken cancellationToken);
        Task<IEnumerable<PlayerSeason>> GetPlayersAsync(int teamId, string seasonId, string gameDate, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetPlayoffTeamsAsync(string seasonId, string gameDate, int round, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetRegularSeasonTeamsAsync(string seasonId, string gameDate, CancellationToken cancellationToken);
        Task<Stream?> GetTeamLogoAsync(int teamId, string gameDate, CancellationToken cancellationToken);
    }
}