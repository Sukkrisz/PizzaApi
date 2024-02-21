namespace Database.Configs
{
    internal static class OrderRepoConstants
    {
        internal static string OrderPizzaUdtName = "OrderPizzaUDT";
        internal static string OrderConnectorId = "OrderId";
        internal static string PizzaConnectorId = "PizzaId";
        internal static string PizzaSizeColumnName = "Size";

        internal static string Sp_CreateOrder = "spOrder_Create";
        internal static string Sp_GetOrder = "spOrder_Get";
        internal static string Sp_GetWithPizzasEf = "spOrder_GetWithPizzasEF";
        internal static string Sp_AttachPizzasToOrder = "spOrder_AddPizzasBulk";
        internal static string Sp_GetPizzasToOrder = "spPizza_GetAllToOrder";
        internal static string Sp_GetPizzasToOrderWithTime = "spOrder_GetMakeTimesByPhoneNumber";
        internal static string Sp_CompleteOrder = "spOrder_Complete";

    }
}
