using Telegram.Bot.Types.ReplyMarkups;

namespace BudgetBot.App.Models
{
    public class InlineReplyModel
    {
        public InlineReplyModel()
        {

        }

        public string Text { get; set; }

        public InlineKeyboardMarkup Markup { get; set; }
    }
}
