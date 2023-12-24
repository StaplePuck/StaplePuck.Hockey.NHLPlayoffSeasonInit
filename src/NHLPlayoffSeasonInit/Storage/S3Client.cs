using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NHLPlayoffSeasonInit.Storage
{
    public class S3Client : IStorageClient
    {
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public S3Client(IOptions<Settings> optins, ILogger<S3Client> logger) 
        { 
            _settings = optins.Value;
            _logger = logger;
        }

        public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken)
        {
            var region = Amazon.RegionEndpoint.USEast1;

            using (var client = new AmazonS3Client(region))
            {
                var request = new ListObjectsRequest
                {
                    BucketName = _settings.S3Bucket,
                    Prefix = path,
                    MaxKeys = 1
                };

                var response = await client.ListObjectsAsync(request, CancellationToken.None);

                return response.S3Objects.Any();
            }
        }

        public async Task UploadAsync(string path, Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                AmazonS3Client s3 = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
                using (TransferUtility tranUtility =
                              new TransferUtility(s3))
                {
                    await tranUtility.UploadAsync(stream, _settings.S3Bucket, path, cancellationToken);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to upload {path}");
            }
        }
    }
}
