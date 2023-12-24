
namespace NHLPlayoffSeasonInit.Storage
{
    public interface IStorageClient
    {
        Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken);
        Task UploadAsync(string path, Stream stream, CancellationToken cancellationToken);
    }
}