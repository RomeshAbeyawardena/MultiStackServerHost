using MultiStackServiceHost.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Shared.Extensions
{
    public static class DescriptorExtensions
    {
        public static IDescriptor<Type> DescribeType<T>(this IDescriptor<Type> descriptor)
        {
            if(descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            return descriptor.Describe(typeof(T));
        }

        public static IDescriptor<Assembly> DescribeAssembly<T>(this IDescriptor<Assembly> descriptor)
        {
            if(descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            return descriptor.Describe(Assembly.GetAssembly(typeof(T)));
        }
    }
}
