namespace MultiStackServiceHost.Domains
{
    public class AppState
    {
        public bool IsRunning { get; set; }
        public bool WarnOnMultipleAbort { get; set; }
        public string DefaultWorkDirectory { get; set; }
    }
}
