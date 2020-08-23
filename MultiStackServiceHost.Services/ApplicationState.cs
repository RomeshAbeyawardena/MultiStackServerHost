using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using System;
using System.Reactive.Subjects;

namespace MultiStackServiceHost.Services
{
    public class ApplicationState : IApplicationState
    {
        public ApplicationState(ISubject<AppState> appStateSubject)
        {
            this.appStateSubject = appStateSubject;
            this.appStateSubject.Subscribe(OnNext);
            appState = new AppState();
        }

        private void OnNext(AppState newState)
        {
            appState = newState;
        }

        void IApplicationState.SetState(Action<AppState> appStateAction)
        {
            appStateAction(appState);
            appStateSubject.OnNext(appState);
        }

        void IApplicationState.Subscribe(Action<AppState> onNextAction)
        {
            appStateSubject.Subscribe(onNextAction);
        }

        AppState IApplicationState.State => appState;

        private readonly ISubject<AppState> appStateSubject;
        private AppState appState;
    }
}
