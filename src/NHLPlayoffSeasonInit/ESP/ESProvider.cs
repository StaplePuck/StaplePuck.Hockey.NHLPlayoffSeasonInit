using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace NHLPlayoffSeasonInit.ESP
{
    public class ESProvider : IESProvider
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;
        private readonly HttpClient _client = new HttpClient();
        private List<ESTeamInfo>? _teamsInfo;
        private readonly Dictionary<string, IEnumerable<ESPPlayerInfo>> _teamRosters = new Dictionary<string, IEnumerable<ESPPlayerInfo>>();

        public ESProvider(IOptions<Settings> options, ILogger<ESProvider> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<string?> GetTeamIdAsync(string commanName, CancellationToken cancellationToken)
        {
            var teamInfo = await GetTeamInfoAsync(commanName, cancellationToken);
            if (teamInfo == null)
            {
                _logger.LogError($"Team info not found {commanName}");
                return null;
            }

            var split = teamInfo.href.Split('/');
            if (split.Length < 2) 
            {
                _logger.LogError($"Team url is not in an expected format {teamInfo.href}");
                return null;
            }
            return string.Join("/", split.TakeLast(2));
        }

        public async Task<string?> GetPlayerIdAsync(string teamId, int playerNumber, CancellationToken cancellationToken)
        {
            var playerInfo = await GetPlayerInfoAsync(teamId, playerNumber, cancellationToken);
            return playerInfo?.id;
        }

        public async Task<Stream?> GetPlayerHeadShotAsync(string teamId, int playerNumber, CancellationToken cancellationToken)
        {
            var playerInfo = await GetPlayerInfoAsync(teamId, playerNumber, cancellationToken);
            if (playerInfo == null)
            {
                _logger.LogError($"Failed to get player info for player {playerNumber}.");
                return null;
            }
            return await _client.GetStreamAsync(playerInfo?.headshot, cancellationToken);
        }

        private async Task<ESTeamInfo?> GetTeamInfoAsync(string teamName, CancellationToken cancellationToken) 
        {
            if (_teamsInfo == null)
            {
                var result = await _client.GetAsync(_settings.ESTeamUrl, cancellationToken);
                var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
                var json = GetJsonResult(stream);

                if (json == null)
                {
                    _logger.LogError($"Unable to parse json for {_settings.ESTeamUrl}");
                    return null;
                }

                var list = new List<ESTeamInfo>();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);
                
                var confrences = doc.Get("page")?.Get("content")?.Get("teams")?.Get("nhl");
                if (confrences == null)
                {
                    _logger.LogError($"Unable to parse team confrences");
                    return null;
                }
                foreach (var confrence in confrences.Value.EnumerateArray())
                {
                    var teams = confrence.Get("teams");
                    if (teams == null)
                    {
                        _logger.LogWarning("Unable to parse teams in confrence");
                        continue;
                    }
                    foreach (var team in teams.Value.EnumerateArray())
                    {
                        var teamInfo = team.Deserialize<ESTeamInfo>();
                        if (teamInfo == null)
                        {
                            _logger.LogWarning("Unable to parse team info");
                            continue;
                        }
                        list.Add(teamInfo);
                    }
                }
                _teamsInfo = list;
            }
            
            return _teamsInfo.FirstOrDefault(x => x.shortName == teamName);
        }

        private async Task<IEnumerable<ESPPlayerInfo>?> GetRoster(string teamId, CancellationToken cancellationToken)
        {
            IEnumerable<ESPPlayerInfo>? roster;
            if (!_teamRosters.TryGetValue(teamId, out roster))
            {
                var url = string.Format(_settings.ESRosterUrl, teamId);
                var result = await _client.GetAsync(url, cancellationToken);
                var stream = await result.Content.ReadAsStreamAsync(cancellationToken);
                var json = GetJsonResult(stream);

                if (json == null)
                {
                    _logger.LogError($"Unable to parse json for {url}");
                    return null;
                }

                var list = new List<ESPPlayerInfo>();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                var rosterGroup = doc.Get("page")?.Get("content")?.Get("roster")?.Get("groups");
                if (rosterGroup == null)
                {
                    _logger.LogError($"Unable to parse team roster");
                    return null;
                }
                foreach (var group in rosterGroup.Value.EnumerateArray())
                {
                    var athletes = group.Get("athletes");
                    if (athletes == null)
                    {
                        _logger.LogWarning("Unable to parse athletes");
                        continue;
                    }
                    foreach (var athlete in athletes.Value.EnumerateArray())
                    {
                        var playerInfo = athlete.Deserialize<ESPPlayerInfo>();
                        if (playerInfo == null)
                        {
                            _logger.LogWarning("Unable to parse player info");
                            continue;
                        }
                        list.Add(playerInfo);
                    }
                }

                roster = list;
                _teamRosters[teamId] = list;
            }

            return roster;
        }

        private async Task<ESPPlayerInfo?> GetPlayerInfoAsync(string teamId, int jerseyNumber, CancellationToken cancellationToken)
        {
            var roster = await GetRoster(teamId, cancellationToken);
            if (roster == null)
            {
                return null;
            }
            return roster.FirstOrDefault(x => x.jersey == jerseyNumber.ToString());
        }

        private string? GetJsonResult(Stream stream)
        {
            var document2 = new HtmlDocument();
            document2.Load(stream);
            var navigator = document2.CreateNavigator();
            
            var scriptsNodes = navigator.Select("/html/body/script");
            while (scriptsNodes.MoveNext())
            {
                if (scriptsNodes.Current != null)
                {
                    var text = scriptsNodes.Current.Value;
                    var index = text.IndexOf("window['__espnfitt__']");
                    if (index != -1)
                    {
                        var json = text.Substring(index + 23).TrimEnd(';');
                        return json;
                    }
                }
            }
            return null;
        }
    }
}
