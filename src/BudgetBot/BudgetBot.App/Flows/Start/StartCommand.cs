using BudgetBot.App.Models;
using MediatR;

namespace BudgetBot.App.Flows.Start
{
    public class StartCommand : IRequest<StartModel>
    {
        public StartCommand(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }
    }
}
