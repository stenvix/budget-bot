namespace BudgetBot.App.Base
{
    public abstract class BaseUserCommand
    {
        protected BaseUserCommand(string username)
        {
            Username = username;
        }

        public string Username { get; }
    }
}
