using Dreamrosia.Koin.Application.Interfaces.Serialization.Options;
using System.Text.Json;

namespace Dreamrosia.Koin.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}