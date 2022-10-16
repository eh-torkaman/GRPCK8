using MainOutsiderService.Services;
using Microsoft.Extensions.Configuration;
namespace MainOutsiderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel((context, options)  =>
            {

                 options.ListenAnyIP(int.Parse(builder.Configuration.GetSection("Ports:Http1").Value) 
                     , c => c.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);

                options.ListenAnyIP(int.Parse(builder.Configuration.GetSection("Ports:Http2").Value),
                      c => c.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);


            });

            builder.Services.AddGrpc(co => { co.IgnoreUnknownServices = true;co.EnableDetailedErrors = true; });
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<StockItemsService>();
             app.MapControllers();
            
             app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}