namespace Models.Db
{
    public static class OrderRepoConstants
    {
        public static string OrderPizzaUdtName = "OrderPizzaUDT";
        public static string OrderConnectorId = "OrderId";
        public static string PizzaConnectorId = "PizzaId";
        public static string PizzaSizeColumnName = "Size";

        public static string Sp_CreateOrder = "spOrder_Create";
        public static string Sp_GetOrder = "spOrder_Get";
        public static string Sp_GetWithPizzasEf = "spOrder_GetWithPizzasEF";
        public static string Sp_AttachPizzasToOrder = "spOrder_AddPizzasBulk";
        public static string Sp_GetPizzasToOrder = "spPizza_GetAllToOrder";
        public static string Sp_GetPizzasToOrderWithTime = "spOrder_GetMakeTimesByPhoneNumber";
        public static string Sp_CompleteOrder = "spOrder_Complete";

    }
}
