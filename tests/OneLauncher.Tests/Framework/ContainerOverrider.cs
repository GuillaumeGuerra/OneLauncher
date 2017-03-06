using System;
using System.Reflection;
using Autofac;
using OneLauncher.Core.Container;

namespace OneLauncher.Tests.Framework
{
    public class ContainerOverrider : IDisposable
    {
        private IContainer _originalContainer;

        private ContainerOverrider(IContainer container)
        {
            _originalContainer = OneLauncherContainer.Instance;

            SetContainer(container);
        }

        public static IDisposable Override(IContainer container)
        {
            return new ContainerOverrider(container);
        }

        public void Dispose()
        {
            // If the container was overriden, we'll dispose it before reverting to the original one
            if (OneLauncherContainer.Instance != null && !ReferenceEquals(OneLauncherContainer.Instance, _originalContainer))
                OneLauncherContainer.Instance.Dispose();

            SetContainer(_originalContainer);
        }

        private void SetContainer(IContainer container)
        {
            typeof(OneLauncherContainer).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public).SetValue(null, container);
        }
    }
}
