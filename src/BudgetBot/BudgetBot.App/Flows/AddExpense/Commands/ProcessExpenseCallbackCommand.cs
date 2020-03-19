using BudgetBot.App.Models;
using MediatR;
using Telegram.Bot.Types;

namespace BudgetBot.App.Flows.AddExpense.Commands
{
    public class ProcessExpenseCallbackCommand : IRequest<InlineReplyModel>
    {
        public ProcessExpenseCallbackCommand(Message message, string data)
        {
            Message = message;
            Data = data;
        }

        public Message Message { get; }
        public string Data { get; }
    }
}
