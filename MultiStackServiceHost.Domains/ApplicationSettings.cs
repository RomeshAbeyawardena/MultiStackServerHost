using Microsoft.Extensions.Configuration;

namespace MultiStackServiceHost.Domains
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }
        public string WarnOnMultipleAbortSetting { get; set; }
        public string DefaultWorkDirectorySetting { get; set; }
        public string ApplicationTitle { get; set; }
        public string WorkingDirectoryParameter { get; set; }
        public string SwitchSeparator { get; set; }
        public string FileName { get; set; }
        public string HelpFile { get; set; }
    }
}
