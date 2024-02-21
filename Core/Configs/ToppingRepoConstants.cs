namespace Database.Configs
{
    internal static class ToppingRepoConstants
    {
        internal static string NameDbColumn = "Name";
        internal static string PriceDbColumn = "Price";
        internal static string ToppingUdtName = "ToppingUDT";

        internal static string Sp_Insert = "dbo.spTopping_Insert";
        internal static string Sp_GetbyId = "dbo.spTopping_Get";
        internal static string Sp_GetAll = "dbo.spTopping_GetAll";
        internal static string Sp_Update = "dbo.spTopping_Update";
        internal static string Sp_Delete = "dbo.spTopping_Delete";
        internal static string Sp_BulkImport = "dbo.spTopping_BulkInsert";
    }
}
