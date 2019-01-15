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
                SeasonId = "20172018",
                SeasonName = "2018 NHL Playoffs R2",
                StartRound = 2
            };
            Updater.PlayOffUpdate(request);
        }
    }
}
