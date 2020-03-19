using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetBot.App.Base;
using BudgetBot.App.Flows.AddExpense.Commands;
using BudgetBot.App.Flows.Start;
using BudgetBot.App.Models;
using BudgetBot.App.Settings;
using BudgetBot.App.State;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BudgetBot.App
{
    public class BotService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IChatStateManager _stateManager;
        private readonly TelegramBotClient _telegramClient;
        private readonly UpdateType[] _updateTypes = { UpdateType.Message, UpdateType.CallbackQuery, UpdateType.InlineQuery, UpdateType.ChosenInlineResult, UpdateType.Unknown };

        public BotService(ILogger logger, IOptionsMonitor<TelegramSettings> settings, IMediator mediator, IChatStateManager stateManager)
        {
            _logger = logger;
            _mediator = mediator;
            _stateManager = stateManager;
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

        private async Task SubscribeAsync(CancellationToken cancellationToken)
        {
            var bot = await _telegramClient.GetMeAsync(cancellationToken);
            _telegramClient.OnMessage += OnMessageReceived;
            _telegramClient.OnCallbackQuery += OnCallbackQueryReceived;
            _telegramClient.OnInlineQuery += OnInlineQueryReceived;
            _telegramClient.OnInlineResultChosen += OnInlineResultChosenReceived;
            _telegramClient.StartReceiving(_updateTypes, cancellationToken);
            _logger.Information($"Start listening for {bot.Username}");
        }

        private async Task Unsubscribe(CancellationToken cancellationToken)
        {
            var bot = await _telegramClient.GetMeAsync(cancellationToken);
            _telegramClient.StopReceiving();
            _telegramClient.OnMessage -= OnMessageReceived;
            _telegramClient.OnCallbackQuery -= OnCallbackQueryReceived;
            _telegramClient.OnInlineQuery -= OnInlineQueryReceived;
            _telegramClient.OnInlineResultChosen -= OnInlineResultChosenReceived;
            _logger.Information($"Stop listening for {bot.Username}");
        }

        private async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (IsBotCommand(message, out var action))
            {
                await this.HandleBotActionAsync(action, message);
                return;
            }

            await this.HandleStateActionAsync(message);
        }


        private async void OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            var message = e.CallbackQuery.Message;
            var state = _stateManager.GetState(message.Chat.Username);

            switch (state.CurrentFlow)
            {
                case FlowType.AddExpense:
                    {
                        var request = new ProcessExpenseCallbackCommand(message, e.CallbackQuery.Data);
                        var reply = await _mediator.Send(request);
                        await this.UpdateReplyAsync(message.Chat.Id, message.MessageId, reply);
                        break;
                    }
            }
        }

        private void OnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {

        }

        private void OnInlineResultChosenReceived(object sender, ChosenInlineResultEventArgs e)
        {

        }

        private bool IsBotCommand(Message message, out string action)
        {
            if (message.Entities == null || message.Entities.Length > 1 || message.Entities.First().Type != MessageEntityType.BotCommand)
            {
                action = string.Empty;
                return false;
            }

            action = message.EntityValues.First();
            return true;
        }

        private async Task HandleBotActionAsync(string action, Message message)
        {
            switch (action)
            {
                case "/start":
                    {
                        var command = new StartCommand(message.Chat.FirstName, message.Chat.LastName);
                        var reply = await _mediator.Send(command);
                        await this.SendReplyAsync(message.Chat.Id, reply);
                        break;
                    }
                case "/add_expense":
                    {
                        var command = new InitializeExpenseCommand(message.Chat.Username);
                        var reply = await _mediator.Send(command);
                        await this.SendReplyAsync(message.Chat.Id, reply);
                        break;
                    }
            }
        }

        private async Task HandleStateActionAsync(Message message)
        {
            var state = _stateManager.GetState(message.Chat.Username);
            switch (state.CurrentFlow)
            {
                case FlowType.AddExpense:
                    {
                        var command = new ProcessExpenseActionCommand(message);
                        var reply = await _mediator.Send(command);
                        await this.UpdateReplyAsync(message.Chat.Id, message.MessageId, reply);
                        break;
                    }
            }
        }

        private async Task SendReplyAsync(long chatId, InlineReplyModel model)
        {
            await _telegramClient.SendTextMessageAsync(chatId, model.Text, replyMarkup: model.Markup);
        }

        private async Task UpdateReplyAsync(long chatId, int messageId, InlineReplyModel reply)
        {
            await _telegramClient.EditMessageTextAsync(chatId, messageId, reply.Text, replyMarkup: reply.Markup);
        }
    }
}
