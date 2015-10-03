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

        protected string Serialize<TInput>(TInput data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            finally
            {
            }
        }

        protected TOutput Deserialize<TOutput>(string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<TOutput>(data);
            }
            finally
            {
            }
        }
    }
}