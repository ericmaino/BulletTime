using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using BulletTime.Controllers;
using BulletTime.Models;
using BulletTime.Networking;
using BulletTime.UI;

namespace BulletTime.ViewModels
{
    public class ServerViewModel : PropertyHost
    {
        private readonly ServerController _controller;
        private readonly CoreDispatcher _dispatcher;

        public ServerViewModel()
        {
            Cameras = new CollectionViewSource();
            Resolutions = new CollectionViewSource();
            _dispatcher = Resolutions.Dispatcher;

            _controller = new ServerController();
            _controller.CamerasChanged += CamerasUpdated;
            _controller.CameraHeartbeat += OnHearbeat;
            Register = new ActionCommand(_controller.RequestReport);
            Record = new ActionCommand(_controller.TriggerRecord);

            SelectedResolution = this.NewProperty(x => x.SelectedResolution);
            SelectedResolution.Changed += UpdateResolution;
        }

        public ICommand Register { get; private set; }
        public ICommand Record { get; private set; }
        public Property<RemoteResolutionModel> SelectedResolution { get; }
        public CollectionViewSource Resolutions { get; }
        public CollectionViewSource Cameras { get; }

        private async void UpdateResolution(IProperty obj)
        {
            await _controller.UpdateResolution(SelectedResolution.Value);
        }

        private async Task CamerasUpdated(IEnumerable<RemoteCameraModel> cameras, IEnumerable<RemoteResolutionModel> resolutions)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Cameras.Source = cameras;
                Resolutions.Source = resolutions;
            });
        }

        private async Task OnHearbeat(CameraHeartBeat heartbeat)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var cameras = (IEnumerable<RemoteCameraModel>) Cameras.Source;
                var camera = cameras.FirstOrDefault(x => x.IPAddress.ToString() == heartbeat.CameraId);

                if (camera != null)
                {
                    if (heartbeat.ViewBuffer.Length > 0)
                    {
                        var decoder = await BitmapDecoder.CreateAsync(heartbeat.ViewBuffer.AsStream().AsRandomAccessStream());
                        var f = await decoder.GetFrameAsync(0);
                        var bmp = new WriteableBitmap((int) f.PixelWidth, (int) f.PixelHeight);
                        await bmp.SetSourceAsync(heartbeat.ViewBuffer.AsStream().AsRandomAccessStream());
                        camera.View.Value = bmp;
                    }


                    camera.CameraState.Value = heartbeat.State;
                }
            });
        }
    }
}