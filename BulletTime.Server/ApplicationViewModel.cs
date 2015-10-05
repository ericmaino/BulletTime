using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using BulletTime.Models;
using BulletTime.Rendering;
using BulletTime.ViewModels;

namespace BulletTime.Server
{
    public class ApplicationViewModel
    {
        private readonly Lazy<ServerViewModel> _server;

        public ApplicationViewModel()
        {
            _server = new Lazy<ServerViewModel>(() => new ServerViewModel());
        }

        public static ApplicationViewModel Current
        {
            get { return (ApplicationViewModel) Application.Current.Resources["ApplicationModel"]; }
        }

        public ServerViewModel ServerViewModel
        {
            get { return _server.Value; }
        }

        public MapViewModel MapViewModel { get; private set; }

        public async Task InitializeMapViewModel()
        {
            var cameras = new List<MapCameraViewModel>();

            foreach (var camera in (IEnumerable<RemoteCameraModel>) ServerViewModel.Cameras.Source)
            {
                cameras.Add(new MapCameraViewModel(camera));
            }

            MapViewModel = new MapViewModel(cameras, 30);
            await Task.Yield();
        }

        public async Task InitializeMapViewModelWithFrames()
        {
            var cameras = new List<MapCameraViewModel>();

            foreach (var camera in (IEnumerable<RemoteCameraModel>) ServerViewModel.Cameras.Source)
            {
                var loader = await ImageLoader.Create(camera);
                cameras.Add(new MapCameraViewModel(camera, await loader.GetFrameImages()));
            }

            MapViewModel = new MapViewModel(cameras, 30);
            MapViewModel.CanvasVisible = Visibility.Collapsed;
            await Task.Yield();
        }
    }
}