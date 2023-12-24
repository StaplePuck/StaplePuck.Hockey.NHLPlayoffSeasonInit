using Microsoft.Extensions.Logging;
using StaplePuck.Core.Client;
using Stats = StaplePuck.Core.Stats;
using System.Dynamic;

namespace NHLPlayoffSeasonInit.StaplePuck
{
    public class StaplePuckProvider : IStaplePuckProvider
    {
        private readonly IStaplePuckClient _staplePuckClient;
        private readonly ILogger _logger;

        private const string SeasonPlayersQuery = @"query my($seasonId: ID) {
  seasons (id: $seasonId) {
    teamSeasons {
      team {
        id
        externalId
        externalId2
      }
    }
    playerSeasons {
      player {
        id
        externalId
        externalId2
        number
      }
      team {
        id
        externalId
        externalId2
      }
    }
  }
}";


        public StaplePuckProvider(IStaplePuckClient staplePuckClient, ILogger<StaplePuckProvider> logger)
        {
            _staplePuckClient = staplePuckClient;
            _logger = logger;
        }

        public async Task<Stats.Season?> GetSesaonPlayersAsync(int seasonId, CancellationToken cancellation)
        {
            var variables = new ExpandoObject() as IDictionary<string, object>;
            variables.Add("seasonId", seasonId);

            var result = await _staplePuckClient.GetAsync<SeasonResponse>(SeasonPlayersQuery, variables);

            if (result.Seasons.Length == 0)
            {
                return null;
            }

            return result.Seasons[0];
        }

        public async Task CreateSeaseonAsync(Season season, CancellationToken cancellationToken)
        {
            var result = await _staplePuckClient.UpdateAsync<Season>("createSeason", season);
        }
    }
}
