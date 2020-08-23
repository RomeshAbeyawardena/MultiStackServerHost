using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class ApplicationStateValueSetter : IApplicationStateValueSetter
    {
        public ApplicationStateValueSetter(
            IApplicationState applicationState,
            ApplicationSettings applicationSettings)
        {
            this.applicationState = applicationState;
            this.applicationSettings = applicationSettings;
            appStateActionswitch = new Switch<string, Func<IApplicationState, string, bool>>()
                .CaseWhen(applicationSettings.WarnOnMultipleAbortSetting, (state, value) => {
                    if(bool.TryParse(value, out var val)){
                        state.SetState(s => s.WarnOnMultipleAbort = val); 
                        return true;
                    }

                    return false;
                })
                .CaseWhen(applicationSettings.DefaultWorkDirectorySetting, (state, value) => {
                    if (string.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
                    {
                        return false;
                    }

                    state.SetState(s => s.DefaultWorkDirectory = value);
                    return true;
                });
        }

        public bool TrySetValue(string setting, string value)
        {
            var appStateSetterAction = appStateActionswitch.Case(setting);

            if(appStateSetterAction == null)
            {
                return false;
            }

            return appStateSetterAction(applicationState, value);
        }

        private readonly ISwitch<string, Func<IApplicationState, string, bool>> appStateActionswitch;
        private readonly IApplicationState applicationState;
        private readonly ApplicationSettings applicationSettings;
    }
}
