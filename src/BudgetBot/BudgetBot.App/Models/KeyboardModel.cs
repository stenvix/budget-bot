using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace BudgetBot.App.Models
{
    public class KeyboardModel
    {
        public KeyboardModel()
        {

        }

        public string Text { get; set; }
        public InlineKeyboardMarkup Markup { get; set; }
    }
}
