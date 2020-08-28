using MultiStackServiceHost.Contracts;
using MultiStackServiceHost.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class FileSystem : IFileSystem
    {
        public IAttempt<string> TryReadTextFile(string path)
        {
            return Attempt.Create(() => File.ReadAllText(path), describer => describer
                .DescribeType<PathTooLongException>()
                .DescribeType<DirectoryNotFoundException>()
                .DescribeType<IOException>()
                .DescribeType<DirectoryNotFoundException>()
                .DescribeType<UnauthorizedAccessException>()
                .DescribeType<FileNotFoundException>()
                .DescribeType<NotSupportedException>()
                .DescribeType<SecurityException>()
            );   
        }

        public IAttempt TryWriteTextFile(string path, string text)
        {
            return Attempt.Create(() => File.WriteAllText(path, text), describer => describer
                .DescribeType<PathTooLongException>()
                .DescribeType<DirectoryNotFoundException>()
                .DescribeType<IOException>()
                .DescribeType<DirectoryNotFoundException>()
                .DescribeType<UnauthorizedAccessException>()
                .DescribeType<FileNotFoundException>()
                .DescribeType<NotSupportedException>()
                .DescribeType<SecurityException>()
            );
        }
    }
}
