using System;
using System.IO;

namespace OneLauncher.Tests.Framework
{
    public class TemporaryDirectory : IDisposable
    {
        public string Location { get; set; }

        public TemporaryDirectory()
        {
            Location = $"Data/{Guid.NewGuid()}";
            Directory.CreateDirectory(Location);
        }

        public void Dispose()
        {
            Directory.Delete(Location, true);
        }
    }
}
