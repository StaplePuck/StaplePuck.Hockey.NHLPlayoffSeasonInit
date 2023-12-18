using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;

namespace NHLPlayoffSeasonInit
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var request = new SeasonRequest
            {
                SeasonId = "20232024",
                GameDate = "2023-12-17",
                SeasonName = "2023 - 2024 NHL Regular Season",
                StartRound = 0
            };
            var tokenSource = new CancellationTokenSource();
            await Updater.UpdateAsync(request, tokenSource.Token);
        }
    }
}
