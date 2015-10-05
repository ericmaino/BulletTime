using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using BulletTime.Models;
using Windows.UI.Xaml;

namespace BulletTime.ViewModels
{
    public class MapViewModel
    {
        public MapViewModel(IEnumerable<MapCameraViewModel> cameras, int frameCount)
        {
            var cameraList = cameras.ToList();
            Cameras = new CollectionViewSource();
            Cameras.Source = cameraList;
            CameraCount = cameraList.Count;
            FrameCount = frameCount;
        }

        public int CameraCount { get; set; }
        public int FrameCount { get; set; }
        public CollectionViewSource Cameras { get; set; }
        public Visibility CanvasVisible { get; set; }
    }
}