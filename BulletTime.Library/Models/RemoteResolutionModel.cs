namespace BulletTime.Models
{
    public class RemoteResolutionModel
    {
        public RemoteResolutionModel(VideoCameraResolutionModel videoResolution)
        {
            Name = videoResolution.ToString();
        }

        public RemoteResolutionModel()
        {
        }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}