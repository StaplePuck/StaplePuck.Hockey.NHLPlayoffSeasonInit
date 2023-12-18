

namespace NHLPlayoffSeasonInit.NHL
{
    public interface INHLProvider
    {
        Task<Franchises.Franchise?> GetFranchiseByIdAsync(int teamId, CancellationToken cancellationToken);
        Task<Franchises.Franchise?> GetFranchiseByNameAsync(string teamName, CancellationToken cancellationToken);
        Task<Roster?> GetRosterByIdAsync(int teamId, string season, string gameDate, CancellationToken cancellationToken);
        Task<Roster> GetRosterByTeamAbbrevAsync(string teamAbbrev, string season, CancellationToken cancellationToken);
        Task<Standings.Standing?> GetStandingByIdAsync(int teamId, string gameDate, CancellationToken cancellationToken);
        Task<Standings.Standing?> GetStandingByNameAsync(string name, string gameDate, CancellationToken cancellationToken);
        Task<IEnumerable<Standings.Standing>> GetStandingsAsync(string gameDate, CancellationToken cancellationToken);
        Task<Stream?> GetTeamLogo(int teamId, string gameDate, CancellationToken cancellationToken);
    }
}