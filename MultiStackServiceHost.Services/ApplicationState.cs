using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using System.Reactive.Subjects;

namespace MultiStackServiceHost.Services
{
    public class ApplicationState : IApplicationState
    {
        public ApplicationState(ISubject<AppState> appState)
        {
            this.appState = appState;
        }

        public void SetRunningState(bool state)
        {
            appState.OnNext(new AppState { IsRunning = state });
        }

        private readonly ISubject<AppState> appState;

    }
}
