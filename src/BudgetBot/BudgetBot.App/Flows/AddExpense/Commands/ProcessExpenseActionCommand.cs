using BudgetBot.App.Models;
using MediatR;
using Telegram.Bot.Types;

namespace BudgetBot.App.Flows.AddExpense.Commands
{
    public class ProcessExpenseActionCommand : IRequest<InlineReplyModel>
    {
        public Message Message { get; }

        public ProcessExpenseActionCommand(Message message)
        {
            Message = message;
        }
    }
}
