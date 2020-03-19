using System.Threading.Tasks;
using BudgetBot.App.Flows.Start;
using BudgetBot.App.Settings;
using BudgetBot.App.State;
using BudgetBot.App.State.Implementations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                services.AddSingleton<IChatStateManager, ChatStateManager>();
                services.Configure<TelegramSettings>(context.Configuration.GetSection("Telegram"));
                services.AddHostedService<BotService>();
                services.AddMediatR(typeof(StartCommand));
                services.AddMemoryCache();
            });

            return hostBuilder;
        }
    }
}
