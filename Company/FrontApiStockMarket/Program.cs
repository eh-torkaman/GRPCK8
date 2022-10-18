using FrontApiStockMarket.Hubs;
using Google.Api;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FrontApiStockMarket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services.AddControllers();
            services.AddDaprClient();
            services.AddSignalR(c => { c.EnableDetailedErrors = true; });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

             services.AddSingleton<RabbitMqManager>();
            
            var app = builder.Build();
            app.UseCors("CorsPolicy");
            app.UseAuthorization();
            app.MapControllers();
            app.UseCloudEvents();

            app.MapSubscribeHandler();
            app.MapHub<StockItemsHub>("/StockItemsHub");

            app.Lifetime.ApplicationStarted.Register(() =>
            {
            });


            using IServiceScope serviceScope = app.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var hostApplicationLifetime = provider.GetRequiredService<IHostApplicationLifetime>();
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                var rabbitMqManager = provider.GetRequiredService<RabbitMqManager>();
                 
            });
            // var r = new RabbitMqManager();
            app.Run();
        }


        
    }
}