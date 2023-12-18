using Microsoft.Extensions.Logging;
using NHLPlayoffSeasonInit.ESP;
using NHLPlayoffSeasonInit.NHL;
using NHLPlayoffSeasonInit.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace NHLPlayoffSeasonInit
{
    public class RosterProvider : IRosterProvider
    {
        private readonly INHLProvider _nhlProvider;
        private readonly IESProvider _esProvider;
        private readonly ILogger _logger;

        public RosterProvider(INHLProvider nhlProvider, IESProvider esProvider, ILogger<RosterProvider> logger)
        {
            _nhlProvider = nhlProvider;
            _esProvider = esProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<int>> GetPlayoffTeamsAsync(string seasonId, string gameDate, int round, CancellationToken cancellationToken)
        {
            var teams = new List<int>();
            var standings = await _nhlProvider.GetStandingsAsync(gameDate, cancellationToken);
            if (standings == null)
            {
                return teams;
            }
            foreach (var standing in standings.Where(x => !string.IsNullOrEmpty(x.clinchIndicator)))
            {
                var franchise = await _nhlProvider.GetFranchiseByNameAsync(standing.teamCommonName._default, cancellationToken);
                if (franchise == null)
                {
                    _logger.LogError($"Unable to get franchine for {standing.teamCommonName._default}");
                }
                else
                {
                    teams.Add(franchise.id);
                }
            }

            return teams;
        }

        public async Task<IEnumerable<int>> GetRegularSeasonTeamsAsync(string seasonId, string gameDate, CancellationToken cancellationToken)
        {
            var teams = new List<int>();
            var standings = await _nhlProvider.GetStandingsAsync(gameDate, cancellationToken);
            if (standings == null)
            {
                return teams;
            }
            foreach (var standing in standings)
            {
                var franchise = await _nhlProvider.GetFranchiseByNameAsync(standing.teamCommonName._default, cancellationToken);
                if (franchise == null)
                {
                    _logger.LogError($"Unable to get franchine for {standing.teamCommonName._default}");
                }
                else
                {
                    teams.Add(franchise.id);
                }
            }

            return teams;
        }

        public async Task<IEnumerable<PlayerSeason>> GetPlayersAsync(int teamId, string seasonId, string gameDate, CancellationToken cancellationToken)
        {
            var roster = await _nhlProvider.GetRosterByIdAsync(teamId, seasonId, gameDate, cancellationToken);
            var standing = await _nhlProvider.GetStandingByIdAsync(teamId, gameDate, cancellationToken);

            var playerList = new List<PlayerSeason>();
            if (roster == null || standing == null)
            {
                _logger.LogError($"Unable to get roster ot standing for team {teamId}");
                return playerList;
            }
            var team = new Team
            {
                ExternalId = teamId,
                FullName = standing.teamName._default,
                LocationName = standing.placeName._default,
                Name = standing.teamCommonName._default,
                ShortName = standing.teamAbbrev._default
            };

            team.ExternalId2 = await _esProvider.GetTeamIdAsync(team.Name, cancellationToken);

            foreach (var item in roster.defensemen)
            {
                playerList.Add(await GeneratePlayerAsync(team, item, DefensemanType(), cancellationToken));
            }
            foreach (var item in roster.forwards)
            {
                playerList.Add(await GeneratePlayerAsync(team, item, ForwardType(), cancellationToken));
            }
            foreach (var item in roster.goalies)
            {
                playerList.Add(await GeneratePlayerAsync(team, item, GoalieType(), cancellationToken));
            }
            return playerList;
        }

        private async Task<PlayerSeason> GeneratePlayerAsync(Team team, Roster.Player player, PositionType position, CancellationToken cancellationToken)
        {
            string? externalId2 = null;
            if (team.ExternalId2 != null)
            {
                externalId2 = await _esProvider.GetPlayerIdAsync(team.ExternalId2, player.sweaterNumber, cancellationToken);
            }
            var playerRequest = new Player()
            {
                FullName = $"{player.firstName._default} {player.lastName._default}",
                ExternalId = player.id.ToString(),
                ExternalId2 = externalId2,
                ShortName = player.lastName._default,
                FirstName = player.firstName._default,
                LastName = player.lastName._default,
                Number = player.sweaterNumber
            };
            var playerSeason = new PlayerSeason
            {
                Player = playerRequest,
                PositionType = position,
                Team = team
            };
            return playerSeason;
        }

        public Task<Stream?> GetTeamLogoAsync(int teamId, string gameDate, CancellationToken cancellationToken)
        {
            return _nhlProvider.GetTeamLogo(teamId, gameDate, cancellationToken);
        }

        public Task<Stream?> GetPlayerHeadShotAsync(PlayerSeason player, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(player.Team?.ExternalId2) || string.IsNullOrEmpty(player.Player?.ExternalId2))
            {
                _logger.LogWarning($"Skipping getting player profile due to ext id not found {player.Player?.FullName}");
                return Task.FromResult<Stream?>(null);
            }
            return _esProvider.GetPlayerHeadShotAsync(player.Team.ExternalId2, player.Player.Number, cancellationToken);
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
