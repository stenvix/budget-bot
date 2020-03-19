using BudgetBot.App.Base;
using BudgetBot.App.Models;
using MediatR;

namespace BudgetBot.App.Flows.AddExpense.Commands
{
    public class InitializeExpenseCommand : BaseUserCommand, IRequest<InlineReplyModel>
    {
        public InitializeExpenseCommand(string username) : base(username)
        {
        }
    }
}
