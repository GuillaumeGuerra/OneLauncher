using System;
using System.Reflection;
using Autofac;

namespace OneLauncher.Tests.Framework
{
    public class ContainerOverrider : IDisposable
    {
        private IContainer _originalContainer;

        private ContainerOverrider(IContainer container)
        {
            _originalContainer = App.Container;

            SetContainer(container);
        }

        public static IDisposable Override(IContainer container)
        {
            return new ContainerOverrider(container);
        }

        public void Dispose()
        {
            // If the container was overriden, we'll dispose it before reverting to the original one
            if (App.Container != null && !ReferenceEquals(App.Container, _originalContainer))
                App.Container.Dispose();

            SetContainer(_originalContainer);
        }

        private void SetContainer(IContainer container)
        {
            typeof(App).GetProperty("Container", BindingFlags.Static | BindingFlags.Public).SetValue(null, container);
        }
    }
}
