using Dapper;
using Data.Db.DbAccess;
using Data.Db.Models.Order;
using Data.Db.Models.Pizza;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Core.Data.Repositories
{
    public class OrderRepo : BaseRepo, IOrderRepo
    {
        private const string ORDER_CONNECTOR_ID = "OrderId";
        private const string PIZZA_CONNECTOR_ID = "PizzaId";
        private const string ORDER_PIZZA_UDT_NAME = "OrderPizzaUDT";

        private const string ADD_PIZZAS_SP_NAME = "spOrder_AddPizzas";
        private const string GET_PIZZAS_SP_NAME = "spPizza_GetAllToOrder";

        private readonly IOptions<ConnectionStringSettings> _settings;

        public OrderRepo(IOptions<ConnectionStringSettings> settings, ISqlDataAccess sqlDataAccess) : base(sqlDataAccess)
        {
            _settings = settings;
            _storedProcs = new Dictionary<CommonDataAccessTypes, string>()
            {
                { CommonDataAccessTypes.Add, "spOrder_Create" },
                { CommonDataAccessTypes.Get, "spOrder_Get" },
                { CommonDataAccessTypes.GetAll, "spOrder_GetWithPizzasEF" }
            };
        }

        public async Task<OrderModel> GetWithPizzas(int orderId)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                var order = await conn.QuerySingleAsync<OrderModel>(
                                                _storedProcs[CommonDataAccessTypes.Get],
                                                new { OrderId = orderId },
                                                commandType: CommandType.StoredProcedure);
                if (order is not null)
                {
                    var pizzas = await _sqlDataAccess.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            GET_PIZZAS_SP_NAME,
                                                                            new { OrderId = order.Id },
                                                                            nameof(PizzaModel.Toppings),
                                                                            conn);
                    order.Pizzas = pizzas?.ToList();
                }

                return order;
            }
        }

        public async Task<OrderModel> GetWithPizzasEF(int orderId)
        {
            var orders = new Dictionary<int, OrderModel>();
            var pizzas = new Dictionary<int, PizzaModel>();

            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                _ = await conn.QueryAsync<OrderModel, AddressModel, PizzaModel, ToppingModel, OrderModel>(
                    sql: "spOrder_GetWithPizzasEF",
                    param: new { OrderId = orderId },
                    map: (order, address, pizza, topping) =>
                    {
                        if (!orders.TryGetValue(order.Id, out var orderEntry))
                        {
                            orderEntry = order;
                            orders.Add(orderEntry.Id, orderEntry);

                            orderEntry.Address = address;
                        }

                        if (!pizzas.TryGetValue(pizza.Id, out var pizzaEntry))
                        {
                            pizzaEntry = pizza;
                            pizzas.Add(pizzaEntry.Id, pizzaEntry);

                            if (orderEntry.Pizzas is null)
                            {
                                orderEntry.Pizzas = new List<PizzaModel>();
                            }

                            orderEntry.Pizzas.Add(pizza);
                        }

                        if (topping.Id != 0)
                        {
                            if (pizzaEntry.Toppings is null)
                            {
                                pizzaEntry.Toppings = new List<ToppingModel>();
                            }

                            pizzaEntry.Toppings.Add(topping);
                        }

                        return orderEntry;
                    });

                orders.TryGetValue(orderId, out var res);
                return res;
            }
        }

        public async Task Create(OrderModel order, int[] pizzaIds)
        {
            var orderInputs = new
            {
                order.PhoneNumber,
                order.OrderDate,
                order.Comment,
                order.Address.City,
                order.Address.Line1,
                order.Address.Line2
            };

            var orderId = await _sqlDataAccess.SaveWithOutput(_storedProcs[CommonDataAccessTypes.Add], orderInputs);

            var orderPizzaConverted = ConvertToInputPizzas(orderId, pizzaIds);
            var pizzaInputs = new
            {
                pizzas = orderPizzaConverted.AsTableValuedParameter(ORDER_PIZZA_UDT_NAME)
            };

            await _sqlDataAccess.SaveDataAsync(ADD_PIZZAS_SP_NAME, pizzaInputs);
        }

        private DataTable ConvertToInputPizzas(int orderId, int[] pizzaIds)
        {
            var res = new DataTable();

            res.Columns.Add(ORDER_CONNECTOR_ID, typeof(int));
            res.Columns.Add(PIZZA_CONNECTOR_ID, typeof(int));

            // Luckily foreach is nowdays as fast, or maybe even faster, as for is ;)
            foreach (var pizzaId in pizzaIds)
            {
                res.Rows.Add(orderId, pizzaId);
            }

            return res;
        }
    }
}
