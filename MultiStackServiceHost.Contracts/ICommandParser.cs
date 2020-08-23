using MultiStackServiceHost.Domains;

namespace MultiStackServiceHost.Contracts
{
    public interface ICommandParser
    {
        Command ParseCommand(string input);
    }
}
