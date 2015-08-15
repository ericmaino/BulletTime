namespace BulletTime.RemoteControl.Commands
{
    public class UploadFrameCommand : BaseCommand<UploadFrameCommand>
    {
        public override string Serialize()
        {
            return Serialize(this);
        }
    }
}