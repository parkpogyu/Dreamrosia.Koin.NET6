using Dreamrosia.Koin.Bot.DTO;
using Dreamrosia.Koin.Bot.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dreamrosia.Koin.Bot.Models
{
    public static class Depot
    {
        public static string UserId => TradingTerms?.UserId;

        public static TradingTermsDto TradingTerms { get; private set; } = new TradingTermsDto();

        public static IEnumerable<SeasonSignalDto> Signals => TradingTerms?.Signals;

        public static IEnumerable<PositionDto> Positions { get; private set; }

        public static IEnumerable<OrderDto> CompletedOrders { get; private set; }

        public static IEnumerable<OrderDto> ThisWeekOrders { get; private set; }

        public static IEnumerable<OrderDto> WaitingOrders { get; private set; }

        public static IEnumerable<OrderDto> StoredOrders
        {
            get
            {
                var orders = new List<OrderDto>();

                orders.AddRange(CompletedOrders);
                orders.AddRange(ThisWeekOrders);
                orders.AddRange(WaitingOrders);

                return orders;
            }
        }

        public static IEnumerable<TransferDto> Deposits { get; private set; }

        public static IEnumerable<TransferDto> Withdraws { get; private set; }

        public static Dictionary<string, TickerDto> Tickers { get; private set; } = new Dictionary<string, TickerDto>();

        public static bool HasFatalError { get; private set; } = false;

        public static void SetTradingTerms(TradingTermsDto tradingTerms)
        {
            if (UserId != tradingTerms?.UserId)
            {
                ClearShelf();
            }

            if (tradingTerms is null) { return; }

            if (HasFatalError && !TradingTerms.UPbitKey.IsOccurredFatalError)
            {
                HasFatalError = false;
            }

            TradingTerms = tradingTerms;

            try
            {
                var removes = (from lt in Tickers.Keys
                               from rt in TradingTerms.Signals.Where(f => f.market.Equals(lt)).DefaultIfEmpty()
                               where rt is null
                               select lt);

                foreach (var remove in removes)
                {
                    Tickers.Remove(remove);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void SetPostions(IEnumerable<PositionDto> positions)
        {
            Positions = positions;
        }

        public static void SetCompletedOrders(IEnumerable<OrderDto> orders)
        {
            var to = Convert.ToDateTime(TradingTerms.LastOrder?.created_at).Date;

            CompletedOrders = orders.Where(f => f.created_at >= to).ToArray();
        }

        public static void SetThisWeekOrders(IEnumerable<OrderDto> orders)
        {
            DateTime first = DateTime.UtcNow.FirstDayOfWeek(DayOfWeek.Monday);

            ThisWeekOrders = orders.Where(f => f.created_at.ToUniversalTime() >= first).ToArray();
        }

        public static void SetWaitingOrders(IEnumerable<OrderDto> orders)
        {
            WaitingOrders = orders;
        }

        public static void SetDeposits(IEnumerable<TransferDto> transfers)
        {
            Deposits = transfers;
        }

        public static void SetWithdraws(IEnumerable<TransferDto> transfers)
        {
            Withdraws = transfers;
        }

        public static void OccurredFatalError()
        {
            HasFatalError = true;

            ClearShelf();
        }

        private static void ClearShelf()
        {
            Positions = new List<PositionDto>();
            CompletedOrders = new List<OrderDto>();
            ThisWeekOrders = new List<OrderDto>();
            WaitingOrders = new List<OrderDto>();
            Deposits = new List<TransferDto>();
            Withdraws = new List<TransferDto>();
        }
    }
}
