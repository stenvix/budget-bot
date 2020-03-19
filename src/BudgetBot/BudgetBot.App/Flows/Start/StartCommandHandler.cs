using System.Threading;
using System.Threading.Tasks;
using BudgetBot.App.Models;
using MediatR;

namespace BudgetBot.App.Flows.Start
{
    public class StartCommandHandler : IRequestHandler<StartCommand, StartModel>
    {
        public async Task<StartModel> Handle(StartCommand request, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
            var model = new StartModel
            {
                Text = this.GetText(request.FirstName, request.LastName)
            };
            return model;
        }

        private string GetText(string firstName, string lastName)
        {
            return $"Hi, {firstName} {lastName}! I'll help you keep track of your income and expenses in real time. Happy expenses!";
        }
    }
}
