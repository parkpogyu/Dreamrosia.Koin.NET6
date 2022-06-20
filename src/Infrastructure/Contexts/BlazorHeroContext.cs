using Dreamrosia.Koin.Application.Interfaces.Services;
using Dreamrosia.Koin.Domain.Contracts;
using Dreamrosia.Koin.Domain.Entities;
using Dreamrosia.Koin.Domain.Enums;
using Dreamrosia.Koin.Infrastructure.Models.Identity;
using Dreamrosia.Koin.Shared.Constants.Application;
using Dreamrosia.Koin.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dreamrosia.Koin.Infrastructure.Contexts
{
    public class BlazorHeroContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public BlazorHeroContext(DbContextOptions<BlazorHeroContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        #region Account
        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Membership> Memberships { get; set; }
        #endregion

        #region Investment
        //public DbSet<Asset> Assets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        #endregion

        #region Market
        public DbSet<Candle> Candles { get; set; }
        public DbSet<Crix> Crixes { get; set; }
        public DbSet<SeasonSignal> SeasonSignals { get; set; }

        public DbSet<Symbol> Symbols { get; set; }
        #endregion

        #region Settlement
        public DbSet<BankingTransaction> BankingTransactions { get; set; }
        public DbSet<Point> Points { get; set; }

        #endregion

        #region Terminal
        public DbSet<MiningBot> MiningBots { get; set; }
        public DbSet<MiningBotTicket> MiningBotTickets { get; set; }
        #endregion

        #region Trading
        public DbSet<ChosenSymbol> ChosenSymbols { get; set; }
        public DbSet<TradingTerms> TradingTerms { get; set; }
        #endregion

        #region UPbit
        public DbSet<UPbitKey> UPbitKeys { get; set; }
        #endregion

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        //entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedOn = _dateTimeService.Now;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        //entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedOn = _dateTimeService.Now;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
            if (_currentUserService.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                                                  .SelectMany(t => t.GetProperties())
                                                  .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);

            builder.Entity<BlazorHeroUser>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");

                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd()
                      .HasMaxLength(36);

                entity.Property(p => p.NickName)
                      .HasMaxLength(50);

                entity.Property(p => p.KoreanName)
                      .HasMaxLength(50);

                entity.Property(p => p.ProfileImage)
                      .HasColumnType("text");

                entity.Property(p => p.IsActive)
                      .HasDefaultValue(true);

                entity.Property(p => p.CreatedBy)
                      .HasMaxLength(36);

                entity.Property(p => p.LastModifiedBy)
                      .HasMaxLength(36);

            });

            builder.Entity<BlazorHeroRole>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");

                entity.Property(p => p.Id)
                      .HasMaxLength(36);

                entity.Property(p => p.Description)
                      .HasMaxLength(256);

                entity.Property(p => p.CreatedBy)
                      .HasMaxLength(36);

                entity.Property(p => p.LastModifiedBy)
                      .HasMaxLength(36);
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");

                entity.Property(p => p.RoleId)
                      .HasMaxLength(36);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");

                entity.Property(p => p.LoginProvider)
                      .HasMaxLength(256);

                entity.Property(p => p.ProviderKey)
                      .HasMaxLength(256);

                entity.Property(p => p.ProviderDisplayName)
                      .HasMaxLength(256);
            });

            builder.Entity<BlazorHeroRoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.Property(p => p.CreatedBy)
                      .HasMaxLength(36);

                entity.Property(p => p.LastModifiedBy)
                      .HasMaxLength(36);

                entity.Property(p => p.RoleId)
                      .HasMaxLength(36);

                entity.HasOne(p => p.Role)
                      .WithMany(p => p.RoleClaims)
                      .HasForeignKey(p => p.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");

                entity.Property(p => p.LoginProvider)
                      .HasMaxLength(256);

                entity.Property(p => p.Name)
                      .HasMaxLength(256);
            });

            #region Account
            builder.Entity<Membership>(entity =>
            {
                entity.ToTable("Memberships").HasComment("회원등급정보");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.Level)
                      .IsRequired(true)
                      .HasDefaultValue(MembershipLevel.Free)
                      .HasComment("회원등급");

                entity.Property(p => p.MaximumAsset)
                      .IsRequired(true)
                      .HasDefaultValue(DefaultValue.TradingTerms.MaximumAsset4Free)
                      .HasComment("최대 운용자산");

                entity.Property(p => p.CommissionRate)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(DefaultValue.Fees.CommissionRate)
                      .HasComment("수수료율");

                entity.Property(p => p.DailyDeductionPoint)
                      .IsRequired(true)
                      .HasDefaultValue(DefaultValue.ChargingPoint.Free)
                      .HasComment("차감포인트");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.Memberships)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            builder.Entity<Subscription>(entity =>
            {
                entity.ToTable("Subscriptions").HasComment("가입정보");

                entity.Property(p => p.Id)
                      .HasMaxLength(36)
                      .HasComment("UserId");

                entity.Property(p => p.GoBoast)
                      .HasDefaultValue(false)
                      .HasComment("자랑하기");

                entity.Property(p => p.RecommenderId)
                      .HasMaxLength(36)
                      .HasComment("추천인 아이디");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithOne(p => p.Subscription)
                      .HasForeignKey<Subscription>(p => p.Id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Recommender as BlazorHeroUser)
                      .WithMany(p => p.Followers)
                      .HasForeignKey(p => p.RecommenderId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
            #endregion

            #region Investment
            builder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders").HasComment("주문내역");

                entity.Property(p => p.Id)
                      .HasMaxLength(36)
                      .HasComment("uuid");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .IsRequired(true)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.market)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("마켓코드");

                entity.Property(p => p.side)
                      .IsRequired(true)
                      .HasComment("주문종류");

                entity.Property(p => p.ord_type)
                      .IsRequired(true)
                      .HasComment("주문방식");

                entity.Property(p => p.price)
                      .HasComment("주문가격");

                entity.Property(p => p.volume)
                      .HasComment("주문수량");

                entity.Property(p => p.amount)
                      .HasComment("주문금액");

                entity.Property(p => p.state)
                      .HasComment("주문상태");

                entity.Property(p => p.executed_volume)
                      .HasComment("체결수량");

                entity.Property(p => p.remaining_volume)
                      .HasComment("잔여수량");

                entity.Property(p => p.reserved_fee)
                      .HasComment("예약 수수료");

                entity.Property(p => p.paid_fee)
                      .HasComment("지불 수수료");

                entity.Property(p => p.remaining_fee)
                      .HasComment("잔여 수수료");

                entity.Property(p => p.locked)
                      .HasComment("묶여있는 비용");

                entity.Property(p => p.trades_count)
                      .HasComment("체결건수");

                entity.Property(p => p.avg_price)
                      .HasComment("평균단가");

                entity.Property(p => p.exec_amount)
                      .HasComment("체결금액");

                entity.Property(p => p.created_at)
                      .IsRequired(true)
                      .HasComment("주문일시");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Position>(entity =>
            {
                entity.ToTable("Positions").HasComment("보유코인");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .IsRequired(true)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.code)
                      .HasMaxLength(10)
                      .IsRequired(true)
                      .HasComment("화폐코드");

                entity.Property(p => p.balance)
                      .IsRequired(true)
                      .HasComment("금액/수량");

                entity.Property(p => p.locked)
                      .HasComment("묶여있는 금액/수량");

                entity.Property(p => p.avg_buy_price)
                      .IsRequired(true)
                      .HasComment("평균단가");

                entity.Property(p => p.avg_buy_price_modified)
                      .IsRequired(true)
                      .HasComment("평균단가 수정 여부");

                entity.Property(p => p.unit_currency)
                      .HasMaxLength(10)
                      .IsRequired(true)
                      .HasComment("기준화폐");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.Positions)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => (new { p.UserId, p.code }))
                      .IsUnique(true);
            });

            builder.Entity<Transfer>(entity =>
            {
                entity.ToTable("Transfers").HasComment("입/출금내역");

                entity.Property(p => p.Id)
                      .HasMaxLength(36)
                      .HasComment("uuid");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .IsRequired(true)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.type)
                      .IsRequired(true)
                      .HasComment("거래종류");

                entity.Property(p => p.code)
                      .HasMaxLength(10)
                      .IsRequired(true)
                      .HasComment("화폐코드");

                entity.Property(p => p.txid)
                      .HasMaxLength(100)
                      .HasComment("거래 요청번호");

                entity.Property(p => p.state)
                      .IsRequired(true)
                      .HasComment("거래상태");

                entity.Property(p => p.created_at)
                      .IsRequired(true)
                      .HasComment("요청일시");

                entity.Property(p => p.done_at)
                      .HasComment("완료일시");

                entity.Property(p => p.amount)
                      .IsRequired(true)
                      .HasComment("금액/수량");

                entity.Property(p => p.fee)
                      .HasComment("수수료");

                entity.Property(p => p.transaction_type)
                      .IsRequired(true)
                      .HasComment("거래유형");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.Transfers)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Market
            builder.Entity<Candle>(entity =>
            {
                entity.ToTable("Candles").HasComment("코인시세");

                entity.Property(p => p.market)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("마켓코드");

                entity.Property(p => p.candle_date_time_utc)
                      .IsRequired(true)
                      .HasComment("기준일시 (UTC)");

                entity.Property(p => p.candle_date_time_kst)
                      .IsRequired(true)
                      .HasComment("기준일시 (KST)");

                entity.Property(p => p.opening_price)
                      .IsRequired(true)
                      .HasComment("시가");

                entity.Property(p => p.high_price)
                      .IsRequired(true)
                      .HasComment("고가");

                entity.Property(p => p.low_price)
                      .IsRequired(true)
                      .HasComment("저가");

                entity.Property(p => p.trade_price)
                      .IsRequired(true)
                      .HasComment("현재가");

                entity.Property(p => p.timestamp)
                      .HasComment("갱신일시");

                entity.Property(p => p.candle_acc_trade_price)
                      .HasComment("누적 거래대금");

                entity.Property(p => p.candle_acc_trade_volume)
                      .HasComment("누적 거래량");

                entity.HasIndex(p => new { p.market, p.candle_date_time_kst })
                      .IsUnique(true);
            });

            builder.Entity<Crix>(entity =>
            {
                entity.ToTable("Crixes").HasComment("시가총액");

                entity.Property(p => p.Id)
                      .HasMaxLength(100)
                      .HasComment("crix_code");

                entity.Property(p => p.koreanName)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .HasComment("한글명");

                entity.Property(p => p.englishName)
                      .HasMaxLength(100)
                      .IsRequired(true)
                      .HasComment("영문명");

                entity.Property(p => p.code)
                      .HasMaxLength(10)
                      .IsRequired(true)
                      .HasComment("화폐코드");

                entity.Property(p => p.unit_currency)
                      .HasMaxLength(10)
                      .IsRequired(true)
                      .HasComment("기준화폐");

                entity.Property(p => p.price)
                      .IsRequired(true)
                      .HasComment("현재가");

                entity.Property(p => p.marketCap)
                      .HasComment("시가총액");

                entity.Property(p => p.accTradePrice24h)
                      .HasComment("거래대금(24H)");

                entity.Property(p => p.signedChangeRate1h)
                      .HasComment("등락률(%) 1H");

                entity.Property(p => p.signedChangeRate24h)
                      .HasComment("등락률(%) 24H");

                entity.Property(p => p.availableVolume)
                      .HasComment("발행량");

                entity.Property(p => p.provider)
                      .HasMaxLength(256)
                      .HasComment("정보제공자");

                entity.Property(p => p.lastUpdated)
                      .IsRequired(true)
                      .HasComment("갱신일시");

                entity.Property(p => p.timestamp)
                      .HasComment("갱신일시");
            });

            builder.Entity<SeasonSignal>(entity =>
            {
                entity.ToTable("SeasonSignals").HasComment("매매신호정보");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.market)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("마켓코드");

                entity.Property(p => p.DailySignal)
                      .IsRequired(true)
                      .HasDefaultValue(Koin.Shared.Enums.SeasonSignals.Indeterminate)
                      .HasComment("일간신호");

                entity.Property(p => p.WeeklySignal)
                      .IsRequired(true)
                      .HasDefaultValue(Koin.Shared.Enums.SeasonSignals.Indeterminate)
                      .HasComment("주간신호");

                entity.Property(p => p.UpdatedAt)
                      .IsRequired(true)
                      .HasComment("갱신일시");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.SeasonSignals)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Symbol)
                      .WithMany(p => p.Signals)
                      .HasForeignKey(p => p.market)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Symbol>(entity =>
            {
                entity.ToTable("Symbols").HasComment("상장코인");

                entity.Property(p => p.Id)
                      .HasMaxLength(20)
                      .HasComment("마켓코드");

                entity.Property(p => p.korean_name)
                      .HasMaxLength(50)
                      .IsRequired(true)
                      .HasComment("한글명");

                entity.Property(p => p.english_name)
                      .HasMaxLength(100)
                      .IsRequired(true)
                      .HasComment("영문명");

                entity.Property(p => p.market_warning)
                      .IsRequired(true)
                      .HasDefaultValue(MarketAlert.None)
                      .HasComment("투자주의");
            });
            #endregion

            #region Settlement
            builder.Entity<BankingTransaction>(entity =>
            {
                entity.ToTable("BankingTransactions").HasComment("입/출금내역");

                entity.Property(p => p.done_at)
                      .IsRequired(true)
                      .HasComment("거래일시");

                entity.Property(p => p.Classification)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("거래구분");

                entity.Property(p => p.Contents)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("기재내용");

                entity.Property(p => p.Deposit)
                      .HasDefaultValue(0)
                      .HasComment("입금금액");

                entity.Property(p => p.Withdraw)
                      .HasDefaultValue(0)
                      .HasComment("출금금액");

                entity.Property(p => p.Balance)
                      .IsRequired(true)
                      .HasComment("잔액");

                entity.Property(p => p.Counterparty)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("상대 예금주명");

                entity.Property(p => p.Memo)
                      .HasMaxLength(20);

                entity.Property(p => p.UserCode)
                      .HasMaxLength(256)
                      .HasComment("회원번호");

                entity.Property(p => p.IsExcluded)
                      .HasDefaultValue(false)
                      .HasComment("정산제외");

                entity.Property(p => p.IsApplied)
                      .HasDefaultValue(false)
                      .HasComment("정산적용");

                entity.HasIndex(p => (new { p.done_at, p.Balance }))
                      .IsUnique(true);
            });

            builder.Entity<Point>(entity =>
            {
                entity.ToTable("Points").HasComment("포인트 정보");

                entity.Property(p => p.TransactionId)
                      .HasComment("입금 거래번호");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.done_at)
                      .IsRequired(true)
                      .HasDefaultValueSql("current_timestamp(6)")
                      .HasComment("생성일시");

                entity.Property(p => p.Membership)
                      .HasDefaultValue(MembershipLevel.Free)
                      .IsRequired(true)
                      .HasComment("회원등급");

                entity.Property(p => p.Type)
                      .HasDefaultValue(PointType.Redeem)
                      .IsRequired(true)
                      .HasComment("거래종류");

                entity.Property(p => p.Amount)
                      .HasDefaultValue(0)
                      .IsRequired(true)
                      .HasComment("충전/차감 포인트");

                entity.Property(p => p.Balance)
                      .HasDefaultValue(0)
                      .IsRequired(true)
                      .HasComment("잔여 포인트");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.Points)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Transaction)
                      .WithOne(p => p.Point)
                      .HasForeignKey<Point>(p => p.TransactionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Terminal
            builder.Entity<MiningBot>(entity =>
            {
                entity.ToTable("MiningBots").HasComment("마이닝 봇");

                entity.Property(p => p.Ticket)
                      .HasMaxLength(36)
                      .HasComment("티켓 아이디");

                entity.Property(p => p.MachineName)
                      .HasMaxLength(256)
                      .HasComment("서버명");

                entity.Property(p => p.Version)
                      .HasMaxLength(20)
                      .HasComment("버전");

                entity.Property(p => p.CurrentDirectory)
                      .HasMaxLength(4096)
                      .HasComment("동작폴더");

                entity.Property(p => p.Touched)
                      .HasComment("갱신일시");

                entity.HasOne(p => p.MiningBotTicket)
                      .WithOne(p => p.MiningBot)
                      .HasForeignKey<MiningBot>(p => p.Ticket)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(p => p.Id)
                      .HasMaxLength(36);
            });

            builder.Entity<MiningBotTicket>(entity =>
            {
                entity.ToTable("MiningBotTickets").HasComment("마이닝 봇 티켓");

                entity.Property(p => p.Id)
                      .HasMaxLength(36);

                entity.Property(p => p.UserId).HasComment("사용자 아이디")
                      .HasMaxLength(36);

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithOne(p => p.MiningBotTicket)
                      .HasForeignKey<MiningBotTicket>(p => p.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

            });
            #endregion

            #region Trading
            builder.Entity<ChosenSymbol>(entity =>
            {
                entity.ToTable("ChosenSymbols").HasComment("매매심볼");

                entity.Property(p => p.UserId)
                      .HasMaxLength(36)
                      .IsRequired(true)
                      .HasComment("사용자 아이디");

                entity.Property(p => p.market)
                      .HasMaxLength(20)
                      .IsRequired(true)
                      .HasComment("마켓코드");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithMany(p => p.ChosenSymbols)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Symbol)
                      .WithMany(p => p.ChosenSymbols)
                      .HasForeignKey(p => p.market)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => (new { p.UserId, p.market }))
                      .IsUnique(true);

            });

            builder.Entity<TradingTerms>(entity =>
            {
                entity.ToTable("TradingTerms").HasComment("매매조건");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithOne(p => p.TradingTerms)
                      .HasForeignKey<TradingTerms>(p => p.Id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Id)
                      .HasMaxLength(36)
                      .HasComment("UserId");

                #region AskTerms
                entity.Property(p => p.UseTakeProfit)
                      .IsRequired(true)
                      .HasDefaultValue(true)
                      .HasComment("수익실현");

                entity.Property(p => p.TakeProfit)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(500F)
                      .HasComment("목표 수익률(%)");

                entity.Property(p => p.UseStopLoss)
                      .IsRequired(true)
                      .HasDefaultValue(true)
                      .HasComment("손실확정");

                entity.Property(p => p.StopLoss)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(-50F)
                      .HasComment("한계 손실륧(%)");

                entity.Property(p => p.UseTrailingStop)
                      .IsRequired(true)
                      .HasDefaultValue(false)
                      .HasComment("추적매도");

                entity.Property(p => p.TrailingStopStart)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(1000F)
                      .HasComment("추적매도 시작 적용률(%)");

                entity.Property(p => p.TrailingStop)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(15F)
                      .HasComment("추적매도 하락 적용률(%)");

                entity.Property(p => p.LiquidatePositions)
                      .IsRequired(true)
                      .HasDefaultValue(false)
                      .HasComment("일괄 매도");
                #endregion

                #region BidTerms
                entity.Property(p => p.AmountOption)
                      .IsRequired(true)
                      .HasDefaultValue(BidAmountOption.Auto)
                      .HasComment("매수금액 옵션");

                entity.Property(p => p.AmountRate)
                      .HasColumnType("float(10,2)")
                      .IsRequired(true)
                      .HasDefaultValue(1F)
                      .HasComment("자산대비 매수금액 비율(%)");

                entity.Property(p => p.Amount)
                      .IsRequired(true)
                      .HasDefaultValue(10000)
                      .HasComment("매수금액");

                entity.Property(p => p.Minimum)
                      .IsRequired(true)
                      .HasDefaultValue(10000)
                      .HasComment("최소 매수금액");

                entity.Property(p => p.Maximum)
                      .IsRequired(true)
                      .HasDefaultValue(0)
                      .HasComment("최대 매수금액");

                entity.Property(p => p.Pyramiding)
                      .HasDefaultValue(false);

                entity.Property(p => p.ApplyMarketPrice)
                      .HasDefaultValue(true);
                #endregion

                #region GeneralTerms
                entity.Property(p => p.TimeFrame)
                      .IsRequired(true)
                      .HasDefaultValue(TimeFrames.Day)
                      .HasComment("적용주기");

                entity.Property(p => p.AutoTrading)
                      .IsRequired(true)
                      .HasDefaultValue(false)
                      .HasComment("자동거래");
                #endregion
            });
            #endregion

            #region UPbit
            builder.Entity<UPbitKey>(entity =>
            {
                entity.ToTable("UPbitKeys").HasComment("업비트 인증키");

                entity.Property(p => p.Id)
                      .HasMaxLength(36)
                      .HasComment("UserId");

                entity.Property(p => p.access_key)
                      .HasMaxLength(256)
                      .IsRequired(true)
                      .HasComment("Access Key");

                entity.Property(p => p.secret_key)
                      .HasMaxLength(256)
                      .IsRequired(true)
                      .HasComment("Secret Key");

                entity.Property(p => p.IsAuthenticated)
                      .IsRequired(true)
                      .HasDefaultValue(false)
                      .HasComment("인증 여부");

                entity.Property(p => p.expire_at)
                      .HasComment("인증 만료일");

                entity.Property(p => p.IsOccurredFatalError)
                      .IsRequired(true)
                      .HasDefaultValue(false)
                      .HasComment("중대오류 여부");

                entity.Property(p => p.FatalError)
                      .HasMaxLength(256)
                      .IsRequired(false)
                      .HasComment("중대오류");

                entity.HasOne(p => p.User as BlazorHeroUser)
                      .WithOne(p => p.UPbitKey)
                      .HasForeignKey<UPbitKey>(p => p.Id)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

        }
    }
}