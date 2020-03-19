using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetBot.App.Base;
using BudgetBot.App.Flows.AddExpense.Commands;
using BudgetBot.App.Flows.AddExpense.Enums;
using BudgetBot.App.Models;
using BudgetBot.App.State;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace BudgetBot.App.Flows.AddExpense.Handlers
{
    public class AddExpenseCommandHandler :
        IRequestHandler<InitializeExpenseCommand, InlineReplyModel>,
        IRequestHandler<ProcessExpenseActionCommand, InlineReplyModel>,
        IRequestHandler<ProcessExpenseCallbackCommand, InlineReplyModel>
    {
        private readonly IChatStateManager _stateManager;
        private readonly IList<string> _accounts = new List<string> { "Cash", "Monobank", "Privat" };
        private readonly IList<string> _categories = new List<string> { "Bills", "Car", "Clothes", "Communications", "Eating out", "Entertainment", "Food", "Gifts", "Health", "House", "Pets" };

        public AddExpenseCommandHandler(IChatStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public Task<InlineReplyModel> Handle(InitializeExpenseCommand request, CancellationToken cancellationToken)
        {
            _stateManager.SetFlow(request.Username, FlowType.AddExpense);
            _stateManager.SetStage(request.Username, (int)AddExpenseStage.SetAccount);

            var model = new InlineReplyModel
            {
                Text = "Please choose account.",
                Markup = new InlineKeyboardMarkup(new[] { _accounts.Select(i => this.GetButton(i, Constants.Buttons.AccountButton)) })
            };

            return Task.FromResult(model);
        }

        public Task<InlineReplyModel> Handle(ProcessExpenseActionCommand request, CancellationToken cancellationToken)
        {
            //TODO: check if this is last stage
            return Task.FromResult(new InlineReplyModel { Text = "Expense successfully added!" });
        }

        public Task<InlineReplyModel> Handle(ProcessExpenseCallbackCommand request, CancellationToken cancellationToken)
        {
            var state = _stateManager.GetState(request.Message.Chat.Username);
            if (!state.CurrentStage.HasValue)
            {
                return Task.FromResult(new InlineReplyModel { Text = "Invalid stage!" });
            }

            InlineReplyModel reply = new InlineReplyModel();

            switch ((AddExpenseStage)state.CurrentStage)
            {
                case AddExpenseStage.SetAccount:
                    {
                        reply = this.SetAccount(request.Message.Chat.Username);
                        break;
                    }
                case AddExpenseStage.SetCategory:
                    {
                        reply = this.SetCategory(request.Message.Chat.Username);
                        break;
                    }
            }

            return Task.FromResult(reply);
        }

        private InlineReplyModel SetAccount(string username)
        {
            //TODO: save account

            var model = new InlineReplyModel
            {
                Text = "Please choose account.",
                Markup = new InlineKeyboardMarkup(new[] { _categories.Select(i => this.GetButton(i, Constants.Buttons.CategoryButton)) })
            };

            _stateManager.SetStage(username, (int)AddExpenseStage.SetCategory);
            return model;
        }

        private InlineReplyModel SetCategory(string username)
        {
            //TODO: save category

            var model = new InlineReplyModel
            {
                Text = "Please enter expense amount:"
            };

            _stateManager.SetStage(username, (int)AddExpenseStage.SetAmount);
            return model;
        }

        private InlineKeyboardButton GetButton(string account, string button)
        {
            return InlineKeyboardButton.WithCallbackData(
                account,
                JsonConvert.SerializeObject(
                    new CallbackDataModel
                    {
                        Button = button,
                        Data = account
                    }));
        }
    }
}
