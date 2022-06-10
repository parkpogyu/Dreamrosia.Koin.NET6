using Dreamrosia.Koin.Application.Enums;

namespace Dreamrosia.Koin.Application.Configurations
{
    public class ServerConfiguration
    {
        public string Secret { get; set; }

        public ServerModes Mode { get; set; }
        public string[] Urls { get; set; }
        public static string DefaultUrl => "http://localhost:5000";
    }
}