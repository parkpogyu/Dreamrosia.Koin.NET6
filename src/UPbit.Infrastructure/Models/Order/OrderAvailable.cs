using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "주문가능정보")]
    public class OrderAvailable
    {
        // 타입          필드                                설명
        //------------------------------------------------------------------------------------
        // NumberString  bid_fee                            매수 수수료 비율 
        // NumberString  ask_fee                            매도 수수료 비율 
        //------------------------------------------------------------------------------------
        // Object        market                             마켓에 대한 정보    
        // String        market.id                          마켓의 유일 키 
        // String        market.name                        마켓 이름 
        // Array[String] market.order_types                 지원 주문 방식    
        // Array[String] market.order_sides                 지원 주문 종류   
        // Object        market.bid                         매수 시 제약사항   
        // String        market.bid.currency                화폐를 의미하는 영문 대문자 코드  
        // String        market.bit.price_unit              주문금액 단위 
        // Number        market.bid.min_total               최소 매도/매수 금액 
        // Object        market.ask                         매도 시 제약사항   
        // String        market.ask.currency                화폐를 의미하는 영문 대문자 코드  
        // String        market.ask.price_unit              주문금액 단위 
        // Number        market.ask.min_total               최소 매도/매수 금액 
        // String        market.max_total                   최대 매도/매수 금액 
        // String        market.state                       마켓 운영 상태    
        //------------------------------------------------------------------------------------
        // Object        bid_account                        매수 시 사용하는 화폐의 계좌 상태 
        // String        bid_account.currency               화폐를 의미하는 영문 대문자 코드 
        // NumberString  bid_account.balance                주문가능 금액/수량 
        // NumberString  bid_account.locked                 주문 중 묶여있는 금액/수량 
        // NumberString  bid_account.avg_buy_price          매수평균가   
        // Boolean       bid_account.avg_buy_price_modified 매수평균가 수정 여부 
        // String        bid_account.unit_currency          평단가 기준 화폐   
        //------------------------------------------------------------------------------------
        // Object        ask_account                        매도 시 사용하는 화폐의 계좌 상태 
        // String        ask_account.currency               화폐를 의미하는 영문 대문자 코드 
        // NumberString  ask_account.balance                주문가능 금액/수량 
        // NumberString  ask_account.locked                 주문 중 묶여있는 금액/수량 
        // NumberString  ask_account.avg_buy_price          매수평균가   
        // Boolean       ask_account.avg_buy_price_modified 매수평균가 수정 여부 
        // String        ask_account.unit_currency          평단가 기준 화폐   
        //------------------------------------------------------------------------------------

        public decimal bid_fee { get; set; }
        public decimal ask_fee { get; set; }
        public Market market { get; set; }
        public Position bid_account { get; set; }
        public Position ask_account { get; set; }

        public static long MinimumBidAmount => 10000;
        public static long MinimumOrderableAmount => 5000;
    }
}
