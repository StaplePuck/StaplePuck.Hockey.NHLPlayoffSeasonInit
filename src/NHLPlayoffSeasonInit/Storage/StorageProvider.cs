using Microsoft.Extensions.Logging;
using NHLPlayoffSeasonInit.Roster;
using Stats = StaplePuck.Core.Stats;

namespace NHLPlayoffSeasonInit.Storage
{
    public class StorageProvider : IStorageProvider
    {
        private readonly IRosterProvider _rosterProvider;
        private readonly IStorageClient _storageClient;
        private readonly ILogger _logger;

        public StorageProvider(IRosterProvider rosterProvider, IStorageClient storageClient, ILogger<StorageProvider> logger)
        {
            _rosterProvider = rosterProvider;
            _storageClient = storageClient;
            _logger = logger;
        }

        public async Task UploadLogoAsync(int teamId, int externalId, string gameDate, bool overwrite, CancellationToken cancellationToken)
        {
            var path = $"logos/{teamId}.svg";
            if (!overwrite)
            {
                if (await _storageClient.FileExistsAsync(path, cancellationToken))
                {
                    _logger.LogInformation($"Logo for team {teamId} already exists, skipping");
                    return;
                }
            }

            var stream = await _rosterProvider.GetTeamLogoAsync(externalId, gameDate, cancellationToken);
            if (stream == null)
            {
                return;
            }

            await _storageClient.UploadAsync(path, stream, cancellationToken);
        }

        public async Task UploadHeadshotAsync(Stats.PlayerSeason playerSeason, bool overwrite, CancellationToken cancellationToken)
        {
            if (playerSeason.Player == null || playerSeason?.Team?.ExternalId2 == null)
            {
                _logger.LogWarning($"No data for player");
                return;
            }
            var path = $"headshots/{playerSeason.Player.Id}.png";
            if (!overwrite)
            {
                if (await _storageClient.FileExistsAsync(path, cancellationToken))
                {
                    _logger.LogInformation($"Headshot for player {playerSeason.Player.Id} already exists, skipping");
                    return;
                }
            }

            var stream = await _rosterProvider.GetPlayerHeadShotAsync(playerSeason.Team.ExternalId2, playerSeason.Player.Number, cancellationToken);
            if (stream == null)
            {
                return;
            }

            await _storageClient.UploadAsync(path, stream, cancellationToken);
        }
    }
}
