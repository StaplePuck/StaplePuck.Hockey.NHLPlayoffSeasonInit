using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using StaplePuck.Core.Stats;
using GraphQL.Client;
using StaplePuck.Core.Client;

namespace NHLPlayoffSeasonInit
{
    public class Updater
    {
        public static void PlayOffUpdate(SeasonRequest request)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<Settings>(configuration.GetSection("Settings"))
                .AddSingleton<StatsProvider>()
                .AddStaplePuckClient(configuration)
                .BuildServiceProvider();

            var stats = serviceProvider.GetService<StatsProvider>();
            IEnumerable<int> teamIds;
            if (request.StartRound > 1)
            {
                teamIds = stats.GetTeamsAtRoundAsync(request.SeasonId, request.StartRound).Result;
            }
            else
            {
                teamIds = stats.GetTeamsAtStartAsync(request.SeasonId).Result;
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
                var players = stats.GetPlayersAsync(request.SeasonId, item).Result;
                season.PlayerSeasons.AddRange(players);
            }

            var client = serviceProvider.GetService<IStaplePuckClient>();
            var result = client.UpdateAsync<Season>("createSeason", season).Result;
        }
    }
}
