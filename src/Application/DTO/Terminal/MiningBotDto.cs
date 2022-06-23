using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    [Display(Name = "마이닝 봇")]
    public class MiningBotDto
    {
        public string Id { get; set; }

        public string MachineName { get; set; }

        public string Version { get; set; }

        public string CurrentDirectory { get; set; }

        [JsonIgnore]
        public string LastDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentDirectory))
                {
                    return string.Empty;
                }
                else
                {
                    var directory = CurrentDirectory.Replace('\\', Path.DirectorySeparatorChar);

                    return Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar));
                }
            }
        }

        public DateTime? Touched { get; set; }

        [JsonIgnore]
        public TimeSpan? Elapsed => Touched is null ? null : DateTime.Now.Subtract(Convert.ToDateTime(Touched));
    }
}