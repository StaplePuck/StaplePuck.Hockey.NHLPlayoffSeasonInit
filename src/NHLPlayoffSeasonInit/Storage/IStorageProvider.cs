using StaplePuck.Core.Stats;

namespace NHLPlayoffSeasonInit.Storage
{
    public interface IStorageProvider
    {
        Task UploadHeadshotAsync(PlayerSeason playerSeason, bool overwrite, CancellationToken cancellationToken);
        Task UploadLogoAsync(int teamId, int externalId, string gameDate, bool overwrite, CancellationToken cancellationToken);
    }
}