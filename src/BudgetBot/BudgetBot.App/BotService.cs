using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetBot.App.Commands;
using BudgetBot.App.Settings;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace BudgetBot.App
{
    public class BotService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly TelegramBotClient _telegramClient;
        private readonly UpdateType[] _updateTypes = { UpdateType.Message };

        public BotService(ILogger logger, IOptionsMonitor<TelegramSettings> settings, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
            _telegramClient = new TelegramBotClient(settings.CurrentValue.Token);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.SubscribeAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.Unsubscribe(cancellationToken);
        }

        private async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var action = e.Message.Text.Split(" ").First();
            switch (action)
            {
                case "/start":
                    {
                        await _telegramClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                        var command = new StartCommand(message.Chat.FirstName, message.Chat.LastName);
                        var model = await _mediator.Send(command);
                        await _telegramClient.SendTextMessageAsync(message.Chat.Id, model.Text, replyMarkup: model.Markup);
                        break;
                    }
            }
        }

        private async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            var bot = await _telegramClient.GetMeAsync(cancellationToken);
            _telegramClient.OnMessage += OnMessageReceived;
            _telegramClient.StartReceiving(_updateTypes, cancellationToken);
            _logger.Information($"Start listening for {bot.Username}");
        }

        private async Task Unsubscribe(CancellationToken cancellationToken)
        {
            var bot = await _telegramClient.GetMeAsync(cancellationToken);
            _telegramClient.StopReceiving();
            _telegramClient.OnMessage -= OnMessageReceived;
            _logger.Information($"Stop listening for {bot.Username}");
        }
    }
}
