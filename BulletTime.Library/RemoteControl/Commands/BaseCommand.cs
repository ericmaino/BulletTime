using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BulletTime.RemoteControl.Commands
{
    public abstract class BaseCommand<T> : IRemoteCameraCommand<T>
    {
        protected BaseCommand()
        {
            Name = GetType().Name;
        }

        [JsonIgnore]
        public string Name { get; }

        public virtual T Parse(StringReader reader)
        {
            return Deserialize<T>(reader.ReadToEnd());
        }

        public virtual Task Execute()
        {
            throw new NotImplementedException();
        }

        public abstract string Serialize();

        protected string Serialize<T>(T data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            finally
            {
            }
        }

        protected T Deserialize<T>(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            finally
            {
            }
        }
    }
}