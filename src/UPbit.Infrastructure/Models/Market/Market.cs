using Dreamrosia.Koin.Shared.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dreamrosia.Koin.UPbit.Infrastructure.Models
{
    [Display(Name = "마켓정보")]
    public class Market
    {
        // 타입          필드                                설명
        //------------------------------------------------------------------------------------
        // Object        market                 마켓에 대한 정보    
        // String        market.id              마켓의 유일 키 
        // String        market.name            마켓 이름 
        // Array[String] market.order_types     지원 주문 방식    
        // Array[String] market.order_sides     지원 주문 종류   
        // Object        market.bid             매수 시 제약사항   
        // String        market.bid.currency    화폐를 의미하는 영문 대문자 코드  
        // String        market.bit.price_unit  주문금액 단위 
        // Number        market.bid.min_total   최소 매도/매수 금액 
        // Object        market.ask             매도 시 제약사항   
        // String        market.ask.currency    화폐를 의미하는 영문 대문자 코드  
        // String        market.ask.price_unit  주문금액 단위 
        // Number        market.ask.min_total   최소 매도/매수 금액 
        // String        market.max_total       최대 매도/매수 금액 
        // String        market.state           마켓 운영 상태    
        //------------------------------------------------------------------------------------

        public string id { get; set; }
        public string name { get; set; }
        public List<OrderType> order_types { get; set; }
        public List<OrderSide> order_sides { get; set; }
        public Restrictions bid { get; set; }
        public Restrictions ask { get; set; }
        public decimal max_total { get; set; }
        public string state { get; set; }
    }
}
