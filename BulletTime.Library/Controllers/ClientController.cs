using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using BulletTime.Models;

namespace BulletTime.Controllers
{
    public class ClientController
    {
        private CameraController _controller;

        public ClientController()
        {
            PrepareForRecording += async () => { await Task.Yield(); };
            RecordingCompleted += async () => { await Task.Yield(); };
            RecordingStarted += async () => { await Task.Yield(); };
        }

        public async Task UpdateCamera(VideoCameraModel model)
        {
            if (_controller != null)
            {
                await _controller.Shutdown();
            }

            if (model == null)
            {
                _controller = null;
            }
            else
            {
                _controller = new CameraController(model);
                await _controller.InitializeModel();
            }
        }

        public IEnumerable<VideoCameraResolutionModel> GetSupportedResolutions()
        {
            ThrowIfCameraIsNull();
            return _controller.GetSupportedResolutions().ToList();
        }

        private void ThrowIfCameraIsNull()
        {
            if (_controller == null)
            {
                throw new InvalidOperationException("No camera has been initialized. Please call UpdateCamera");
            }
        }

        public async Task Shutdown()
        {
            ThrowIfCameraIsNull();
            await _controller.Shutdown();
        }

        public async Task SetResolution(VideoCameraResolutionModel resolution)
        {
            ThrowIfCameraIsNull();

            if (resolution.IsRemote)
            {
                resolution = GetSupportedResolutions().First(x => x.GetHashCode() == resolution.GetHashCode());
            }

            await _controller.SetResolution(resolution.Properties);
            await _controller.StartPreview();
        }

        public async Task<IRandomAccessStream> Snapshot()
        {
            return (await _controller.Record(1)).First();
        }

        public async Task<IEnumerable<IRandomAccessStream>> Record(int frameCount)
        {
            await _controller.StartPreview();
            await PrepareForRecording();
            await RecordingStarted();
            var frames = await _controller.Record(frameCount);
            await RecordingCompleted();

            return frames;
        }

        public event Func<Task> PrepareForRecording;
        public event Func<Task> RecordingStarted;
        public event Func<Task> RecordingCompleted;
    }
}