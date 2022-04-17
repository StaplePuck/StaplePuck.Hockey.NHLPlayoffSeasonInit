using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using StaplePuck.Core.Stats;

namespace NHLPlayoffSeasonInit
{
    public class StatsProvider
    {
        private readonly Settings _settings;
        private readonly HttpClient _client = new HttpClient();

        public StatsProvider(IOptions<Settings> options)
        {
            _settings = options.Value;
        }

        public async Task<IEnumerable<int>> GetTeamsAtStartAsync(string seasonId)
        {
            var filterList = new List<int>();

            var standingsUrl = string.Format(_settings.StandingsUrl, seasonId);
            var standingsResult = await _client.GetAsync(standingsUrl);
            var standings = await this.SerializeResult<Data.Standings.Result>(standingsResult);
            // Get the wild card teams
            foreach (var item in standings.records.Where(x => x.standingsType.Equals("wildCard", StringComparison.CurrentCultureIgnoreCase)))
            {
                foreach (var team in item.teamRecords.Where(x => x.clinchIndicator != null))
                {
                    filterList.Add(team.team.id);
                }
            }

            // get the rest
            foreach (var item in standings.records.Where(x => x.standingsType.Equals("divisionLeaders", StringComparison.CurrentCultureIgnoreCase)))
            {
                foreach (var team in item.teamRecords.Where(x => x.clinchIndicator != null))
                {
                    filterList.Add(team.team.id);
                }
            }

            return filterList;
        }

        public async Task<IEnumerable<int>> GetTeamsAtRoundAsync(string seasonId, int startRound)
        {
            var filterList = new List<int>();

            var url = string.Format(_settings.PlayoffStandingsUrl, seasonId);
            var dateResult = await _client.GetAsync(url);

            if (!dateResult.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await dateResult.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<Data.Tournament.Result>(content);

            var round = value.rounds.SingleOrDefault(x => x.number == startRound - 1);
            if (round == null)
            {
                return null;
            }
            foreach (var series in round.series)
            {
                if (series.matchupTeams != null)
                {
                    foreach (var item in series.matchupTeams)
                    {
                        if (item.seriesRecord.wins == round.format.numberOfWins)
                        {
                            filterList.Add(item.team.id);
                        }
                    }
                }
            }

            return filterList;
        }

        public async Task<IEnumerable<PlayerSeason>> GetPlayersAsync(string seasonId, int teamId)
        {
            var teamUrl = string.Format(_settings.TeamUrl, teamId);
            var teamResult = await _client.GetAsync(teamUrl);

            var teamResponse = await SerializeResult<Data.TeamInfo.Result>(teamResult);
            var teamInfo = teamResponse.teams[0];
            var team = new Team
            {
                ExternalId = teamInfo.id,
                FullName = teamInfo.name,
                LocationName = teamInfo.locationName,
                Name = teamInfo.teamName, 
                ShortName = teamInfo.abbreviation
            };


            var playerList = new List<PlayerSeason>();

            var url = string.Format(_settings.RosterUrl, teamId, seasonId);
            var rosterResult = await _client.GetAsync(url);

            if (!rosterResult.IsSuccessStatusCode)
            {
                return null;
            }

            var response = await SerializeResult<Data.RosterInfo.Result>(rosterResult);

            foreach (var item in response.roster)
            {
                PositionType position = null;
                if (item.position.type == "Forward")
                {
                    position = ForwardType();
                }
                else if (item.position.type == "Defenseman")
                {
                    position = DefensemanType();
                }
                else if (item.position.type == "Goalie")
                {
                    position = GoalieType();
                }
                int jerseyNumber = -1;
                int.TryParse(item.jerseyNumber, out jerseyNumber);
                var player = new Player()
                {
                    FullName = item.person.fullName,
                    ExternalId = item.person.id.ToString(),
                    ShortName = item.person.lastName,
                    FirstName = item.person.firstName,
                    LastName = item.person.lastName,
                    Number = jerseyNumber
                };
                var playerSeason = new PlayerSeason
                {
                    Player = player,
                    PositionType = position,
                    Team = team
                };
                playerList.Add(playerSeason);
            }
            return playerList;
        }

        private async Task<T> SerializeResult<T>(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = await message.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(content);
            return result;
        }

        private static PositionType DefensemanType()
        {
            return new PositionType { Name = "Defenseman" };
        }

        private static PositionType ForwardType()
        {
            return new PositionType { Name = "Forward" };
        }

        private static PositionType GoalieType()
        {
            return new PositionType { Name = "Goalie" };
        }
    }
}
