namespace Dreamrosia.Koin.Application.DTO
{
    public class FollowerDto : BoasterDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        #region TradingTerms
        public bool AutoTrading { get; set; }
        #endregion

        #region MiningBot
        public bool IsAssignedBot { get; set; }
        #endregion
    }
}
