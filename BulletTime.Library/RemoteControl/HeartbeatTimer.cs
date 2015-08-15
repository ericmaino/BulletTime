using System;
using System.Threading;
using System.Threading.Tasks;

namespace BulletTime.RemoteControl
{
    public class HeartbeatTimer
    {
        private readonly Timer _timer;
        private Func<Task> _heartbeatAction;

        public HeartbeatTimer(Func<Task> onHeartbeat)
        {
            _timer = new Timer(OnHeartbeat, null, Timeout.Infinite, Timeout.Infinite);
            _heartbeatAction = onHeartbeat;
        }

        public async void OnHeartbeat(object unused)
        {
            try
            {
                await _heartbeatAction();
            }
            catch
            {
            }
        }

        public void Enable(TimeSpan interval)
        {
            _timer.Change(TimeSpan.Zero, interval);
        }

        public void Disable()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void UpdateCallback(Func<Task> action)
        {
            _heartbeatAction = action;
        }
    }
}