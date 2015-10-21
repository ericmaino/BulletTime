using BulletTime.Controllers;
using BulletTime.Models;
using BulletTime.RemoteControl;
using BulletTime.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BulletTime.ViewModels
{
    public class CameraClientViewModel : PropertyHost
    {
        private readonly ClientController _controller;
        private readonly CameraClientRemoteControl _remote;

        public CameraClientViewModel()
        {
            FramesProcessed += async f => { await Task.Yield(); };
            Cameras = new CollectionViewSource();
            Resolutions = new CollectionViewSource();
            Dispatcher = Cameras.Dispatcher;

            CurrentImage = this.NewProperty(x => x.CurrentImage);
            SelectedCamera = this.NewProperty(x => x.SelectedCamera);
            SelectedResolution = this.NewProperty(x => x.SelectedResolution);
            Countdown = this.NewProperty(x => x.Countdown);
            CountdownVisible = this.NewProperty(x => x.CountdownVisible);
            RecordingVisibility = this.NewProperty(x => x.RecordingVisibility);
            CountdownVisible.Value = Visibility.Collapsed;
            RecordingVisibility.Value = Visibility.Collapsed;

            _controller = new ClientController();
            _controller.PrepareForRecording += PrepareForRecording;
            _controller.RecordingStarted += RecordingStarted;
            _controller.RecordingCompleted += RecordingCompleted;
            Record = new ActionCommand(RecordVideo);

            SelectedCamera.Changed += SelectedCameraChanged;
            SelectedResolution.Changed += SelectedResolutionChanged;
            _remote = new CameraClientRemoteControl(this, _controller);
        }

        public CoreDispatcher Dispatcher { get; }
        public Property<int> Countdown { get; }
        public Property<Visibility> CountdownVisible { get; }
        public Property<Visibility> RecordingVisibility { get; }
        public Property<DeviceInformation> SelectedCamera { get; }
        public Property<VideoCameraResolutionModel> SelectedResolution { get; }
        public Property<ImageSource> CurrentImage { get; }
        public MediaCapture Media { get; set; }
        public CollectionViewSource Resolutions { get; }
        public CollectionViewSource Cameras { get; }
        public ICommand Record { get; set; }
        public event Func<IEnumerable<IRandomAccessStream>, Task> FramesProcessed;

        private async Task RecordVideo()
        {
            var frames = await _controller.Record(30);
            await FramesProcessed(frames);
        }

        public async void Initialize()
        {
            await DetectCameras();
            await _remote.Initialize();
        }

        private async Task DetectCameras()
        {
            Debug.WriteLine("Detecting Cameras");
            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await DetectCameras(); });
                return;
            }

            if (this.SelectedCamera.Value == null)
            {
                var cameras = await CameraController.GetCameras();

                if (!cameras.Any())
                {
                    var t = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                        await DetectCameras();

                    });
                };

                Cameras.Source = cameras;
            }
        }

        private async void SelectedCameraChanged(IProperty obj)
        {
            if (SelectedCamera.Value == null)
            {
                await _controller.Shutdown();
            }
            else
            {
                var model = new VideoCameraModel(SelectedCamera.Value);
                await _controller.UpdateCamera(model);
                Media = model.Media;
                Resolutions.Source = _controller.GetSupportedResolutions();
            }
        }

        private async Task RecordingCompleted()
        {
            RecordingVisibility.Value = Visibility.Collapsed;
            await Task.Yield();
        }

        private async Task RecordingStarted()
        {
            RecordingVisibility.Value = Visibility.Visible;
            await Task.Yield();
        }

        private async Task PrepareForRecording()
        {
            for (var i = 1; i >= 0; i--)
            {
                Countdown.Value = i;
                CountdownVisible.Value = Visibility.Visible;

                if (i > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.25));
                }
            }

            CountdownVisible.Value = Visibility.Collapsed;
        }

        private async void SelectedResolutionChanged(IProperty obj)
        {
            if (SelectedResolution.Value != null)
            {
                // Need to debounce
                await _controller.SetResolution(SelectedResolution.Value);
            }
        }
    }
}