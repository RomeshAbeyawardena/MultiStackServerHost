using MultiStackServiceHost.Domains;
using System;

namespace MultiStackServiceHost.Contracts
{
    public interface IApplicationState
    {
        void Subscribe(Action<AppState> onNextAction);
        void SetState(Action<AppState> appStateAction);
        AppState State { get; }
    }
}
