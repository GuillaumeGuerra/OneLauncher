using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace OneLauncher.Core.Container
{
    public static class DependencyInjectionExtensions
    {
        public static IEnumerable<TService> GetImplementations<TService>(this IContainer container)
        {
            return container.Resolve<IEnumerable<TService>>();
        }

        public static IEnumerable<Type> GetRegisteredSubClasses<TBaseType>(this IContainer container)
        {
            return container.ComponentRegistry.Registrations
                .Where(r => typeof(TBaseType).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType);
        }
    }
}
