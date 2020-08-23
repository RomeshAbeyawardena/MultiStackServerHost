using MultiStackServiceHost.Domains;
using System;

namespace MultiStackServiceHost.Contracts
{
    public interface ICommandActions : ISwitch<string, Action<Command>>
    {
        
    }
}
