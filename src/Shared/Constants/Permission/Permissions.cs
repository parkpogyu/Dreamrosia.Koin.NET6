using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dreamrosia.Koin.Shared.Constants.Permission
{
    public static class Permissions
    {
        public static class AuditTrails
        {
            public const string View = "Permissions.AuditTrails.View";
            public const string Export = "Permissions.AuditTrails.Export";
        }

        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }

        #region Identity
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Export = "Permissions.Users.Export";
        }

        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
        }

        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Delete = "Permissions.RoleClaims.Delete";
        }
        #endregion

        #region Market
        public static class Symbols
        {
            public const string View = "Permissions.Symbols.View";
            public const string Export = "Permissions.Symbols.Export";
        }

        public static class Candles
        {
            public const string View = "Permissions.Candles.View";
            public const string Export = "Permissions.Candles.Export";
        }
        #endregion

        #region Investment
        public static class Assets
        {
            public const string View = "Permissions.Assets.View";
            public const string Export = "Permissions.Assets.Export";
        }

        public static class Orders
        {
            public const string View = "Permissions.Orders.View";
            public const string Export = "Permissions.Orders.Export";
        }

        public static class Positions
        {
            public const string View = "Permissions.Positions.View";
            public const string Export = "Permissions.Positions.Export";
        }

        public static class Transfers
        {
            public const string View = "Permissions.Transfers.View";
            public const string Export = "Permissions.Transfers.Export";
        }
        #endregion

        #region Order
        public static class TradingTerms
        {
            public const string View = "Permissions.TraingTerms.View";
            public const string Edit = "Permissions.TraingTerms.Edit";
        }

        public static class UPbitKeys
        {
            public const string View = "Permissions.UPbitKeys.View";
            public const string Edit = "Permissions.UPbitKeys.Edit";
        }
        #endregion

        #region Mock
        public static class MockTradings
        {
            public const string BackTest = "Permissions.MockTradings.BackTest";
            public const string RealTest = "Permissions.MockTradings.RealTest";
        }
        #endregion

        #region Terminal
        public static class MiningBots
        {
            public const string View = "Permissions.MiningBots.View";
            public const string Edit = "Permissions.MiningBots.Edit";
            public const string Export = "Permissions.MiningBots.Export";
        }
        #endregion

        #region Settlement
        public static class BankingTransactions
        {
            public const string View = "Permissions.BankingTransactions.View";
            public const string Edit = "Permissions.BankingTransactions.Edit";
            public const string UserView = "Permissions.BankingTransactions.UserView";
            public const string Export = "Permissions.BankingTransactions.Export";
            public const string Import = "Permissions.BankingTransactions.Import";
        }

        public static class Points
        {
            public const string View = "Permissions.Points.View";
            //public const string UserView = "Permissions.Points.UserView";
            //public const string Export = "Permissions.Points.Export";
        }
        #endregion

        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permssions = new List<string>();

            foreach (var prop in typeof(Permissions).GetNestedTypes()
                                                    .SelectMany(c => c.GetFields(BindingFlags.Public |
                                                                                 BindingFlags.Static |
                                                                                 BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);

                if (propertyValue is null) { continue; }

                permssions.Add(propertyValue.ToString());
            }

            return permssions;
        }
    }
}