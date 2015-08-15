using BulletTime.Models;

namespace BulletTime.RemoteControl.Commands
{
    public class UpdateResolutionCommand : BaseCommand<RemoteResolutionModel>
    {
        private readonly RemoteResolutionModel _payload;

        public UpdateResolutionCommand()
        {
        }

        public UpdateResolutionCommand(RemoteResolutionModel model)
        {
            _payload = model;
        }

        public override string Serialize()
        {
            return base.Serialize(_payload);
        }
    }
}