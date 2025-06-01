using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using CreateExcelFileWorkerService.Services;

namespace CreateExcelFileWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // context.Configuration vasitəsilə IConfiguration əldə edilir
                    IConfiguration configuration = context.Configuration;

                    var uri = configuration.GetSection("RabbitMQ")["URI"];

                    services.AddSingleton(new ConnectionFactory
                    {
                        Uri = new Uri(uri),
                        DispatchConsumersAsync = true // Asinxron consumer işlətmək üçün
                    });
                    services.AddSingleton<RabbitMQClientService>();
                    services.AddSingleton<CarService>();
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}
