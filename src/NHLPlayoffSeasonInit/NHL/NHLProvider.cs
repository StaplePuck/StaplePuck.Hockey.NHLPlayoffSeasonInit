using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace NHLPlayoffSeasonInit.NHL
{
    public class NHLProvider : INHLProvider
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly HttpClient _client = new HttpClient();
        private IEnumerable<Franchises.Franchise>? _franchises;
        private IEnumerable<Standings.Standing>? _standings;
        private readonly Dictionary<string, Roster> _rosters = new Dictionary<string, Roster>();

        public NHLProvider(IOptions<Settings> options, ILogger<NHLProvider> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<Franchises.Franchise?> GetFranchiseByNameAsync(string teamName, CancellationToken cancellationToken)
        {
            var franchises = await GetFranchisesAsync(cancellationToken);
            return franchises.FirstOrDefault(x => x.teamCommonName == teamName);
        }

        public async Task<Franchises.Franchise?> GetFranchiseByIdAsync(int teamId, CancellationToken cancellationToken)
        {
            var franchises = await GetFranchisesAsync(cancellationToken);
            return franchises.FirstOrDefault(x => x.id == teamId);
        }

        private async Task<IEnumerable<Franchises.Franchise>> GetFranchisesAsync(CancellationToken cancellationToken)
        {
            if (null == _franchises)
            {
                //https://api.nhle.com/stats/rest/en/franchise
                var result = await _client.GetAsync(_settings.FranchiseUrl, cancellationToken);
                var franchises = await DeserializeResult<Franchises>(result, cancellationToken);
                if (null == franchises)
                {
                    _logger.LogError("Failed to get franchise result");
                    return new Franchises.Franchise[0];
                }

                _franchises = franchises.data;
            }

            return _franchises;
        }

        public async Task<IEnumerable<Standings.Standing>> GetStandingsAsync(string gameDate, CancellationToken cancellationToken)
        {
            if (null == _standings)
            {
                //https://api-web.nhle.com/v1/standings/2023-04-14
                var result = await _client.GetAsync(string.Format(_settings.StandingsUrl, gameDate), cancellationToken);
                var standings = await DeserializeResult<Standings>(result, cancellationToken);
                if (null == standings)
                {
                    _logger.LogError("Failed to get standings result");
                    return new Standings.Standing[0];
                }

                _standings = standings.standings;
            }

            return _standings;
        }

        public async Task<Standings.Standing?> GetStandingByNameAsync(string name, string gameDate, CancellationToken cancellationToken)
        {
            var standings = await GetStandingsAsync(gameDate, cancellationToken);
            return standings.FirstOrDefault(x => x.teamCommonName._default == name);
        }

        public async Task<Standings.Standing?> GetStandingByIdAsync(int teamId, string gameDate, CancellationToken cancellationToken)
        {
            var franchise = await GetFranchiseByIdAsync(teamId, cancellationToken);
            if (franchise == null)
            {
                _logger.LogError($"Failed to get team by id {teamId}");
                return null;
            }

            return await GetStandingByNameAsync(franchise.teamCommonName, gameDate, cancellationToken);
        }

        public async Task<Roster> GetRosterByTeamAbbrevAsync(string teamAbbrev, string season, CancellationToken cancellationToken)
        {
            Roster? roster;
            if (!_rosters.TryGetValue(teamAbbrev, out roster))
            {
                //https://api-web.nhle.com/v1/roster/PIT/20232024
                var result = await _client.GetAsync(string.Format(_settings.RosterUrl, teamAbbrev, season), cancellationToken);
                roster = await DeserializeResult<Roster>(result, cancellationToken);
                if (null == roster)
                {
                    _logger.LogError($"Failed to get roster for {teamAbbrev}");
                    return new Roster();
                }

                _rosters[teamAbbrev] = roster;
            }

            return roster;
        }

        public async Task<Roster?> GetRosterByIdAsync(int teamId, string season, string gameDate, CancellationToken cancellationToken)
        {
            var standing = await GetStandingByIdAsync(teamId, gameDate, cancellationToken);
            if (standing == null)
            {
                _logger.LogError($"Failed to get standing by id {teamId}");
                return null;
            }

            return await GetRosterByTeamAbbrevAsync(standing.teamAbbrev._default, season, cancellationToken);
        }

        public async Task<Stream?> GetTeamLogo(int teamId, string gameDate, CancellationToken cancellationToken)
        {
            var standing = await GetStandingByIdAsync(teamId, gameDate, cancellationToken);
            if (standing == null)
            {
                _logger.LogError($"Failed to get standing for logo {teamId}");
                return null;
            }

            return await _client.GetStreamAsync(standing.teamLogo);
        }

        private async Task<T?> DeserializeResult<T>(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (!message.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = await message.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(content);
            return result;
        }
    }
}
