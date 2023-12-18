using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StaplePuck.Core.Auth;
using GraphQL.Client;
using StaplePuck.Core.Client;
using NHLPlayoffSeasonInit.Request;
using NHLPlayoffSeasonInit.ESP;
using NHLPlayoffSeasonInit.NHL;

namespace NHLPlayoffSeasonInit
{
    public class Updater
    {
        public static async Task UpdateAsync(SeasonRequest request, CancellationToken cancellationToken)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<Settings>(configuration.GetSection("Settings"))
                .AddSingleton<IRosterProvider, RosterProvider>()
                .AddSingleton<INHLProvider, NHLProvider>()
                .AddSingleton<IESProvider, ESProvider>()
                .AddAuth0Client(configuration)
                .AddStaplePuckClient(configuration)
                .AddLogging()
                .BuildServiceProvider();

            var provider = serviceProvider.GetRequiredService<IRosterProvider>();
            IEnumerable<int> teamIds;
            if (request.StartRound == 0)
            {
                //teamIds = new int[]{ 6, 14, 15, 4, 5, 8, 12, 3, 2, 13, 10, 29, 19, 21, 54, 25, 22, 16, 18, 53, 23, 30, 20, 52};
                //teamIds = new int[] { 8, 12, 2, 16, 53, 23, 20, 25, 19, 6, 15, 54, 21, 4, 14, 29  };
                teamIds = await provider.GetRegularSeasonTeamsAsync(request.SeasonId, request.GameDate, cancellationToken);
            }
            else
            {
                teamIds = await provider.GetPlayoffTeamsAsync(request.SeasonId, request.GameDate, request.StartRound, cancellationToken);
            }

            var sport = new Sport { Name = "Hockey" };
            var season = new Season
            {
                ExternalId = request.SeasonId,
                FullName = request.SeasonName,
                Sport = sport,
                IsPlayoffs = true,
                StartRound = request.StartRound,
                PlayerSeasons = new List<PlayerSeason>()
            };
            foreach (var item in teamIds)
            {
                var players = await provider.GetPlayersAsync(item, request.SeasonId, request.GameDate, cancellationToken);
                season.PlayerSeasons.AddRange(players);
            }

            var client = serviceProvider.GetRequiredService<IStaplePuckClient>();
            //var types = client.GetAsync<StaplePuck.Core.Fantasy.User>("user").Result;
            var result = client.UpdateAsync<Season>("createSeason", season).Result;
         }
    }
}
