using BudgetBot.App.Base;

namespace BudgetBot.App.State
{
    public interface IChatStateManager
    {
        IChatState GetState(string key);

        void SetFlow(string key, FlowType flow);

        void SetStage(string key, int stage);
    }
}
