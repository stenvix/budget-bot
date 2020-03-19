using BudgetBot.App.Base;
using Microsoft.Extensions.Caching.Memory;

namespace BudgetBot.App.State.Implementations
{
    public class ChatStateManager : IChatStateManager
    {
        private readonly IMemoryCache _memoryCache;

        public ChatStateManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IChatState GetState(string key)
        {
            return _memoryCache.GetOrCreate(key, this.InitializeState);
        }

        public void SetFlow(string key, FlowType flow)
        {
            var state = this.GetState(key);
            state.SetFlow(flow);
        }

        public void SetStage(string key, int stage)
        {
            var state = this.GetState(key);
            state.SetStage(stage);
        }

        private IChatState InitializeState(ICacheEntry cacheEntry)
        {
            var state = new ChatState();
            state.SetFlow(FlowType.NotStarted);
            cacheEntry.Value = state;

            return state;
        }
    }
}
