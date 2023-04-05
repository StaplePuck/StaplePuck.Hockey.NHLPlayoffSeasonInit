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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            

            var request = new SeasonRequest
            {
                SeasonId = "20222023",
                SeasonName = "2023 NHL Playoffs",
                StartRound = 1
            };
            Updater.PlayOffUpdate(request);
        }
    }
}
