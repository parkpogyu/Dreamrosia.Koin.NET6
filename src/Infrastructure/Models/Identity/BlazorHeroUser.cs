using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamrosia.Koin.Infrastructure.Models.Identity
{
    public class BlazorHeroUser : IdentityUser<string>, IDomainUser, IAuditableEntity<string>
    {
        public string NickName { get; set; }

        public string KoreanName { get; set; }

        [Column(TypeName = "text")]
        public string ProfileImage { get; set; }

        public bool IsActive { get; set; } = false;

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Membership> Memberships { get; set; }

        public Subscription Subscription { get; set; }
        public ICollection<Subscription> Followers { get; set; }

        public MiningBotTicket MiningBotTicket { get; set; }

        public UPbitKey UPbitKey { get; set; }

        public TradingTerms TradingTerms { get; set; }

        public ICollection<ChosenSymbol> ChosenSymbols { get; set; }

        public ICollection<SeasonSignal> SeasonSignals { get; set; }

        public ICollection<Transfer> Transfers { get; set; }

        public ICollection<Order> Orders { get; set; }

        public ICollection<Position> Positions { get; set; }

        public ICollection<Point> Points { get; set; }

        public BlazorHeroUser()
        {
        }
    }
}