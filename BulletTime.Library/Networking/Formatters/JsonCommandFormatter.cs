using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using BulletTime.Models;
using Newtonsoft.Json;

namespace BulletTime.Networking.Formatters
{
    public class JsonCommandFormatter : ISocketStreamFormatter<CommandJson>
    {
        public async Task<CommandJson> Read(IDataReader reader)
        {
            await reader.LoadAsync(sizeof (uint));
            var strSize = reader.ReadUInt32();
            await reader.LoadAsync(strSize);
            return JsonConvert.DeserializeObject<CommandJson>(reader.ReadString(strSize));
        }

        public Task Write(IDataWriter writer, CommandJson dataFrame)
        {
            var content = JsonConvert.SerializeObject(dataFrame);
            writer.WriteUInt32(writer.MeasureString(content));
            writer.WriteString(content);
            return Task.FromResult(0);
        }
    }
}