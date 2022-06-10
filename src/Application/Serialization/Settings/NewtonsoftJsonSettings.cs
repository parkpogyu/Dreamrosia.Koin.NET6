
using Dreamrosia.Koin.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace Dreamrosia.Koin.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}