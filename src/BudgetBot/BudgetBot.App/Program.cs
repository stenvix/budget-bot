using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using BudgetBot.App.Commands;
using BudgetBot.App.Settings;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;

namespace BudgetBot.App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.Configure<TelegramSettings>(context.Configuration.GetSection("Telegram"));
                services.AddHostedService<BotService>();
                services.AddMediatR(typeof(StartCommand));
            });

            return hostBuilder;
        }
    }
}
