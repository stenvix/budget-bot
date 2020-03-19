using System;
using System.Collections.Generic;
using BudgetBot.App.Base;

namespace BudgetBot.App.State.Implementations
{
    public class ChatState : IChatState
    {
        private Dictionary<string, object> _data;

        public ChatState()
        {
            CurrentFlow = FlowType.NotStarted;
            _data = new Dictionary<string, object>();
        }

        public FlowType CurrentFlow { get; private set; }

        public int? CurrentStage { get; private set; }

        public void SetFlow(FlowType flow)
        {
            CurrentFlow = flow;
        }

        public void SetStage(int stage)
        {
            CurrentStage = stage;
        }

        public TStage GetStage<TStage>(TStage defaultValue) where TStage : struct, IComparable
        {
            if (!typeof(TStage).IsEnum) throw new ArgumentException("T must be an enumerated type");
            if (!CurrentStage.HasValue) return defaultValue;

            foreach (TStage item in Enum.GetValues(typeof(TStage)))
            {
                if (item.ToString().Equals(CurrentStage.ToString())) return item;
            }

            return defaultValue;
        }
    }
}
