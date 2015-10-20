using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using BulletTime.Models;

namespace BulletTime.Controllers
{
    public class CameraController
    {
        private readonly ManualResetEvent _cameraOn;
        private readonly VideoCameraModel _model;
        private readonly ManualResetEvent _recording;
        private bool _isInitialized;

        public CameraController(VideoCameraModel model)
        {
            _model = model;
            _recording = new ManualResetEvent(false);
            _cameraOn = new ManualResetEvent(false);
        }

        private bool IsInPreview
        {
            get { return _cameraOn.WaitOne(TimeSpan.FromSeconds(0.25)); }
        }

        public static async Task<IEnumerable<DeviceInformation>> GetCameras()
        {
            var result = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            return result.ToList();
        }

        public async Task InitializeModel()
        {
            if (!_isInitialized)
            {
                await _model.Media.InitializeAsync(new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                    PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                    AudioDeviceId = string.Empty,
                    VideoDeviceId = _model.Camera.Id
                });

                _isInitialized = true;
            }
        }

        internal async Task Shutdown()
        {
            while (_recording.WaitOne(TimeSpan.FromSeconds(0.1)))
            {
                await Task.Delay(500);
            }

            await StopPreview();
        }

        public IEnumerable<VideoCameraResolutionModel> GetSupportedResolutions()
        {
            var result = Enumerable.Empty<VideoCameraResolutionModel>();

            if (_isInitialized)
            {
                result = _model.Media.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview)
                    .Cast<VideoEncodingProperties>()
                    .Where(v => v.Subtype.Equals("YUY2", StringComparison.OrdinalIgnoreCase))
                    .Where(v => GetHertz(v.FrameRate) == 20)
                    .Where(v => v.Width > 400)
                    .Where(v => v.Width < 600)
                    .OrderBy(r => r.Width)
                    .ThenBy(r => r.Height)
                    .ThenBy(r => GetHertz(r.FrameRate))
                    .ThenBy(r => r.Bitrate)
                    .Select(v => new VideoCameraResolutionModel(v))
                    .ToList();
            }

            return result;
        }

        private uint GetHertz(MediaRatio ratio)
        {
            return ratio.Numerator/ratio.Denominator;
        }

        public async Task StartPreview()
        {
            if (!_isInitialized)
            {
                await InitializeModel();
            }

            if (!IsInPreview)
            {
                try
                {
                    var media = _model.Media;
                    var video = media.VideoDeviceController;

                    await video.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, _model.Resolution);
                    await media.StartPreviewAsync();

                    SetDefaults(video.Brightness, true);
                    SetDefaults(video.Contrast, true);
                    SetDefaults(video.Focus, true);
                    SetDefaults(video.Exposure, true);
                    SetDefaults(video.WhiteBalance, true);
                    SetDefaults(video.Hue, true);

                    while (media.CameraStreamState != CameraStreamState.Streaming)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.25));
                    }

                    _cameraOn.Set();
                }
                catch (Exception)
                {
                    Debug.WriteLine("Failed to start preview");
                }
            }
        }

        public async Task StopPreview()
        {
            try
            {
                if (IsInPreview)
                {
                    await _model.Media.StopPreviewAsync();
                }
            }
            finally
            {
                _cameraOn.Reset();
            }
        }

        public async Task<IEnumerable<IRandomAccessStream>> Record(int framesToRecord)
        {
            var frameQueue = new Queue<InMemoryRandomAccessStream>();

            if (!_recording.WaitOne(TimeSpan.FromSeconds(0.5)))
            {
                _recording.Set();

                try
                {
                    if (!IsInPreview)
                    {
                        await StartPreview();
                    }

                    var media = _model.Media;

                    while (frameQueue.Count < framesToRecord)
                    {
                        var stream = new InMemoryRandomAccessStream();
                        await media.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                        frameQueue.Enqueue(stream);
                    }
                }
                finally
                {
                    _recording.Reset();
                }
            }

            return frameQueue;
        }

        private void SetDefaults(MediaDeviceControl control, bool useAuto = false)
        {
            if (control.Capabilities.Supported)
            {
                if (useAuto)
                {
                    control.TrySetAuto(useAuto);
                }
                else
                {
                    control.TrySetAuto(useAuto);
                    control.TrySetValue(control.Capabilities.Default);
                }
            }
        }

        public async Task SetResolution(VideoEncodingProperties resolution)
        {
            var startPreview = IsInPreview;

            if (IsInPreview)
            {
                await StopPreview();
            }

            _model.Resolution = resolution;

            if (startPreview)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.25));
                await StartPreview();
            }
        }
    }
}