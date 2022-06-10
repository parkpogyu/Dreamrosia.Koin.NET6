using System.ComponentModel;

namespace Dreamrosia.Koin.Shared.Enums
{
    public enum BidAmountOption
    {
        [Description("고정금액")]
        Fixed,

        [Description("수동비율(%)")]
        Manual,

        [Description("자동비율(%)")]
        Auto,
    }

    public enum OrderSide
    {
        [Description("매도")]
        ask,

        [Description("매수")]
        bid,

        [Description("보유")]
        hold,
    }

    public enum OrderType
    {
        [Description("지정가")]
        limit,

        [Description("시장가(매수)")]
        price,

        [Description("시장가(매도)")]
        market,
    }

    public enum OrderState
    {
        [Description("대기")]
        wait,

        [Description("예약")]
        watch,

        [Description("완료")]
        done,

        [Description("취소")]
        cancel,
    }

    public enum OrderReason
    {
        [Description("미지정")]
        None,

        [Description("신호")]
        Signal,

        [Description("손실")]
        StopLoss,

        [Description("수익")]
        TakeProfit,

        [Description("MDD")]
        MDD,

        [Description("T/S")]
        TrailingStop,

        [Description("추가매수")]
        Pyramiding,

        [Description("청산")]
        Liquidate
    }

    public enum OrderBy
    {
        [Description("오름차순")]
        asc,

        [Description("내림차순")]
        desc,
    }

    public enum TransferType
    {
        [Description("출금")]
        withdraw,

        [Description("입금")]
        deposit,
    }

    public enum TransferState
    {
        [Description("접수중")]
        submitting,

        [Description("접수완료")]
        submitted,

        //[Description("입금 대기중")]
        //[Description("출금 대기중")]
        [Description("대기중")]
        almost_accepted,

        [Description("출금대기")]
        waiting,

        [Description("거부")]
        rejected,

        [Description("실패")]
        failed,

        [Description("완료")]
        accepted,

        [Description("처리중")]
        processing,

        [Description("완료")]
        done,

        [Description("취소")]
        canceled,

        #region withdraws
        // - submitting : 처리 중
        // - submitted : 처리완료
        // - almost_accepted : 출금대기중
        // - rejected : 거부
        // - accepted : 승인됨
        // - processing : 처리 중
        // - done : 완료
        // - canceled : 취소됨
        #endregion

        #region Deposit
        // - submitting : 처리 중
        // - submitted : 처리완료
        // - almost_accepted : 입금 대기 중
        // - rejected : 거절
        // - accepted : 승인됨
        // - done : 완료
        // - processing : 처리 중
        #endregion
    }

    public enum TransferTransaction
    {
        [Description("일반")]
        @default,

        [Description("즉시")]
        @internal,
    }

    public enum TimeFrames
    {
        [Description("분간")]
        Minute = 1,

        [Description("일간")]
        Day,

        [Description("주간")]
        Week,

        [Description("월간")]
        Month,

        [Description("년간")]
        Year,
    }

    public enum MinutesUnit
    {
        [Description("1분")]
        _1 = 1,

        [Description("3분")]
        _3 = 3,

        [Description("5분")]
        _5 = 5,

        [Description("10분")]
        _10 = 10,

        [Description("15분")]
        _15 = 15,

        [Description("30분")]
        _30 = 30,

        [Description("60분")]
        _60 = 60,

        [Description("240분")]
        _240 = 240,
    }

    public enum TickerDirection
    {
        [Description("보합")]
        Even,

        [Description("상승")]
        Rise,

        [Description("하락")]
        Fall
    }

    public enum WalletState
    {
        [Description("입출금 가능")]
        working,

        [Description("출금만 가능")]
        withdraw_only,

        [Description("입금만 가능")]
        deposit_only,

        [Description("입출금 중단")]
        paused,

        [Description("미지원")]
        unsupported,
    }

    public enum BlockState
    {
        [Description("미정의")]
        None,

        [Description("정상")]
        normal,

        [Description("지연")]
        delayed,

        [Description("비활성")]
        inactive,
    }

    public enum MarketAlert
    {
        [Description("정상")]
        None = 0,

        [Description("주의")]
        Caution,

        [Description("경고")]
        Warning,

        [Description("위험")]
        Risk,
    }

    public enum SeasonSignals
    {
        [Description("미정")]
        Indeterminate = 0,

        [Description("동일")]
        Equal,

        [Description("봄")]
        GoldenCross,

        [Description("여름")]
        Above,

        [Description("가을")]
        DeadCross,

        [Description("겨울")]
        Below,
    }

    public enum BasePrices
    {
        [Description("시가")]
        Open,

        [Description("저가")]
        Low,

        [Description("고가")]
        High,

        [Description("종가")]
        Close
    }
}
