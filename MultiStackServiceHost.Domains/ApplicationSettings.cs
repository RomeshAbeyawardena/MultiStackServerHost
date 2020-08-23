using Microsoft.Extensions.Configuration;

namespace MultiStackServiceHost.Domains
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration configuration)
        {
            configuration.Bind(this);
        }

        public string WorkingDirectoryParameter { get; set; }
        public string SwitchSeparator { get; set; }
        public string FileName { get; set; }
        public string HelpFile { get; set; }
    }
}
