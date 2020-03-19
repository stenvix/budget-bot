using System;
using BudgetBot.App.Base;

namespace BudgetBot.App.State
{
    public interface IChatState
    {
        FlowType CurrentFlow { get; }

        int? CurrentStage { get; }

        public void SetFlow(FlowType flow);

        void SetStage(int stage);

        TStage GetStage<TStage>(TStage defaultValue) where TStage : struct, IComparable;
    }
}
