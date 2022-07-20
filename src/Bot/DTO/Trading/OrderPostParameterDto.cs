using Dreamrosia.Koin.UPbit.Infrastructure.Clients;
using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Bot.DTO
{
    public class OrderPostParameterDto : ExOrderPost.ExParameter
    {
        [JsonIgnore]
        public string Remark { get; set; }

        [JsonIgnore]
        public double BalEvalAmt { get; set; }  
    }
}
