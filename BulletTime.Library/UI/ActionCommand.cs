using System;
using System.Threading.Tasks;

namespace BulletTime.UI
{
    public class ActionCommand : AsyncCommand
    {
        private readonly Func<Task> _action;

        public ActionCommand(Func<Task> action)
        {
            _action = action;
        }

        protected override async Task ExecuteAsync()
        {
            await _action();
        }
    }
}