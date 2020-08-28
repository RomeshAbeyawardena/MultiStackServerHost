using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Contracts
{
    public interface IFileSystem
    {
        IAttempt TryWriteTextFile(string path, string text);
        IAttempt<string> TryReadTextFile(string path);
    }
}
