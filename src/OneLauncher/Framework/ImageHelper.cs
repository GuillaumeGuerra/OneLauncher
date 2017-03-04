using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OneLauncher.Properties;

namespace OneLauncher.Framework
{
    public static class ImageHelper
    {
        public static ImageSource ToImageSource(this Icon image)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                      image.ToBitmap().GetHbitmap(),
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
