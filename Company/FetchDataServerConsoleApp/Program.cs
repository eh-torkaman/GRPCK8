using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using SharedData.proto;
using Grpc.Net.Compression;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FetchDataServerConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var host = CreateDefaultBuilder().Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var hostApplicationLifetime = provider.GetRequiredService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                var fetchService = provider.GetRequiredService<FetchDataService>();
                fetchService.FetchStockItemsCurrentPrice();
                fetchService.FetchStockItems();
            });
            host.Run();
        }
        static IHostBuilder CreateDefaultBuilder()
        {

            return Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(app =>
                {
                    app.AddEnvironmentVariables();
                    app.AddJsonFile("appsettings.json");
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<RabbitMqManager>();
                    services.AddSingleton<FetchDataService>();
                });




        }

    }

}