using System.Collections.Generic;
using Autofac;

namespace OneLauncher.Framework
{
    public static class DependencyInjectionExtensions
    {
        public static IEnumerable<TService> GetImplementations<TService>(this IContainer container)
        {
            return container.Resolve<IEnumerable<TService>>();
        }
    }
}
