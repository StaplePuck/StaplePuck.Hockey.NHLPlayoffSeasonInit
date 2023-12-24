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
using NHLPlayoffSeasonInit.ESP;
using NHLPlayoffSeasonInit.NHL;
using NHLPlayoffSeasonInit.Roster;
using NHLPlayoffSeasonInit.Storage;
using NHLPlayoffSeasonInit.StaplePuck;

namespace NHLPlayoffSeasonInit
{
    public class Updater
    {
        private static ServiceProvider GenerateServices()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            return new ServiceCollection()
                .AddOptions()
                .Configure<Settings>(configuration.GetSection("Settings"))
                .AddSingleton<IRosterProvider, RosterProvider>()
                .AddSingleton<INHLProvider, NHLProvider>()
                .AddSingleton<IESProvider, ESProvider>()

                .AddSingleton<IStaplePuckProvider, StaplePuckProvider>()

                .AddSingleton<IStorageClient, S3Client>()
                .AddSingleton<IStorageProvider, StorageProvider>()

                .AddAuth0Client(configuration)
                .AddStaplePuckClient(configuration)
                .AddLogging()
                .BuildServiceProvider();
        }

        public static async Task UpdateAsync(SeasonRequest request, CancellationToken cancellationToken)
        {
            var serviceProvider = GenerateServices();

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

            var staplePuck = serviceProvider.GetRequiredService<IStaplePuckProvider>();
            await staplePuck.CreateSeaseonAsync(season, cancellationToken);
        }


        public static async Task UpdateAssetsAsync(AssetRequest request, CancellationToken cancellationToken)
        {
            var serviceProvider = GenerateServices();

            var provider = serviceProvider.GetRequiredService<IStaplePuckProvider>();
            var storageProvider = serviceProvider.GetRequiredService<IStorageProvider>();
            var season = await provider.GetSesaonPlayersAsync(request.SeasonId, cancellationToken);

            if (season == null)
            {
                Console.Error.WriteLine("No season info");
                return;
            }
            foreach (var teamSeason in season.TeamSeasons)
            {
                if (teamSeason?.Team != null)
                {
                    var team = teamSeason.Team;
                    await storageProvider.UploadLogoAsync(team.Id, team.ExternalId, request.GameDate, request.OverwriteLogos, cancellationToken);
                }
            }

            foreach (var playerSeason in season.PlayerSeasons)
            {
                await storageProvider.UploadHeadshotAsync(playerSeason, request.OverwriteHeadshots, cancellationToken);
            }
        }
    }
}
