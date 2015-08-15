using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using BulletTime.Models;

namespace BulletTime.ViewModels
{
    public class MapViewModel
    {
        public MapViewModel()
        {
            Cameras = new CollectionViewSource();
            IEnumerable<MapCameraViewModel> query = null;

            if (CurrentCameras != null)
            {
                query = CurrentCameras.Select(x => new MapCameraViewModel(x));
            }

            var y = query.ToList();
            Cameras.Source = y;
            CameraCount = y.Count;
            FrameCount = 30;
        }

        public static IEnumerable<RemoteCameraModel> CurrentCameras { get; set; }
        public int CameraCount { get; set; }
        public int FrameCount { get; set; }
        public CollectionViewSource Cameras { get; set; }
    }
}