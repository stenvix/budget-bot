using System.Threading;
using System.Threading.Tasks;
using BudgetBot.App.Commands;
using BudgetBot.App.Models;
using MediatR;
using Telegram.Bot.Types.ReplyMarkups;

namespace BudgetBot.App.Handlers
{
    public class StartCommandHandler : IRequestHandler<StartCommand, StartModel>
    {
        public async Task<StartModel> Handle(StartCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
            var model = new StartModel
            {
                Text = this.GetText(request.FirstName, request.LastName),
                Markup = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData("Start", "Data"),
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Some", "query")
                })
            };
            return model;
        }

        private string GetText(string firstName, string lastName)
        {
            return $"Hi, {firstName} {lastName}! I'll help you keep track of your income and expenses in real time. Please, choose command from list below.";
        }
    }
}
