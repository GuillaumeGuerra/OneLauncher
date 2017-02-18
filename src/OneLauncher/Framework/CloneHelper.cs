using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Infragistics.Windows.DataPresenter;

namespace OneLauncher.Framework
{
    public static class CloneHelper
    {
        public static T DeepClone<T>(this T item)
        {
            using (var stream1 = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(stream1, item);

                stream1.Seek(0, SeekOrigin.Begin);

                return (T) serializer.ReadObject(stream1);
            }
        }
    }
}
