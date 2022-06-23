using System.Text.Json.Serialization;

namespace Dreamrosia.Koin.Application.DTO
{
    public class UserFullInfoDto : UserDto
    {
        public SubscriptionDto Subscription { get; set; }
        public TradingTermsDto TradingTerms { get; set; }
        public MiningBotTicketDto MiningBotTicket { get; set; }
        public UPbitKeyDto UPbitKey { get; set; }

        #region Role
        public string RolesDescription { get; set; }
        #endregion

        [JsonIgnore]
        public bool ShowDetails { get; set; }
    }
}