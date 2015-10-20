using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace BulletTime.Server
{
    public class RenderViewModel
    {
        static RenderViewModel()
        {
            Instance = new RenderViewModel();
        }

        public RenderViewModel()
        {
            if (Instance != null)
            {
                Images = Instance.Images;
            }
            else
            {
                Images = new List<WriteableBitmap>();
            }
        }

        public static RenderViewModel Instance { get; set; }
        public IList<WriteableBitmap> Images { get; set; }
    }
}