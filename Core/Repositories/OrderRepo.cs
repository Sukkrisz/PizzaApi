using Dapper;
using Database.Configs;
using Database.DbAccess;
using Database.Helpers;
using Database.Models.Order;
using Database.Models.Pizza;
using Database.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelLibrary.Shared.Func;
using System.Data;

namespace Database.Repositories
{
    public sealed class OrderRepo : BaseRepo, IOrderRepo
    {
        private readonly IOptions<ConnectionStringSettings> _settings;
        private readonly ILogger _logger;

        public OrderRepo(IOptions<ConnectionStringSettings> settings, ISqlDataAccess sqlDataAccess, ILogger<OrderRepo> logger) : base(sqlDataAccess)
        {
            _settings = settings;
            _logger = logger;
        }

        // Running the query this way returns the order (and it's detail to the app server's memory only once)
        // after that we'll return only the pizzas belonging to the given order.
        // It costs us two queries but spares us all the repetion of the order's columns for each topping row

        // Data rerutned thisway:
        // 1 | +36408873611 | 2021.03.15. 20:40 | test comment | Budapest | Petőfy 13 | 401/A | 1 (this is the order status, it will be mapped to Statusenum)
        // Szalámis | 2300 | 2 (size) | Sajt | 20 (seconds to add to a normal size pizza)
        // Szalámis | 2300 | 2 | Paradicsomszósz | 15
        // Szalámis | 2300 | 2 | Szalámi | 10
        // It could be further optimized by retrieving the pizza details only once as well and then connection all the toppings to it.
        // That would spare the repetition of the pizza details as well.
        // Please compare this to the EF way
        public async Task<OrderModel> GetWithPizzas(int orderId)
        {
            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                var order = await conn.QuerySingleAsync<OrderModel>(
                                                OrderRepoConstants.Sp_GetOrder,
                                                new { OrderId = orderId },
                                                commandType: CommandType.StoredProcedure);
                if (order is not null)
                {
                    var pizzas = await _sqlDataAccess.LoadDataMultiObjectAsync<OrderedPizzaWithDetails, ToppingModel, dynamic>(
                                                                            OrderRepoConstants.Sp_GetPizzasToOrder,
                                                                            new { OrderId = order.Id },
                                                                            nameof(PizzaModel.Toppings),
                                                                            conn);
                    order.Pizzas = pizzas?.ToList();
                }

                return order;
            }
        }

        // This is how EF would return the values. It would come back in one query, but with a more difficult query in the db (more joins)
        // + lots more data. The order's details would be reperated with each topping row

        // Data returned:
        // 1 | +36408873611 | 2021.03.15. 20:40 | test comment | Budapest | Petőfy 13 | 401/A | 1 | Szalámis | 2300 | Sajt | 20 (seconds to add to a normal size pizza)
        // 1 | +36408873611 | 2021.03.15. 20:40 | test comment | Budapest | Petőfy 13 | 401/A | 1 | Szalámis | 2300 | Paradicsomszósz | 15
        // 1 | +36408873611 | 2021.03.15. 20:40 | test comment | Budapest | Petőfy 13 | 401/A | 1 | Szalámis | 2300 | Szalámi | 10
        public async Task<OrderModel> GetWithPizzasEF(int orderId)
        {
            var orders = new Dictionary<int, OrderModel>();
            var pizzas = new Dictionary<int, OrderedPizzaWithDetails>();

            using (IDbConnection conn = new SqlConnection(_settings.Value.PizzaDb))
            {
                // This will return 1 row for all toppings (having all the data for the pizza and the topping as well.
                _ = await conn.QueryAsync<OrderModel, AddressModel, OrderedPizzaWithDetails, ToppingModel, OrderModel>(
                    sql: OrderRepoConstants.Sp_GetWithPizzasEf,
                    param: new { OrderId = orderId },
                    map: (order, address, pizza, topping) =>
                    {
                        // Store the first occurence of the order. After that only new ones would be stored, but here
                        // we will only find one order, since the query filters all the other out
                        if (!orders.TryGetValue(order.Id, out var orderEntry))
                        {
                            orderEntry = order;
                            orders.Add(orderEntry.Id, orderEntry);

                            orderEntry.Address = address;
                        }

                        // Store the first appearance. After that we'll get the mapped pizza object out of the dictionary and
                        // connect all the found toppings to the previously stored entity
                        if (!pizzas.TryGetValue(pizza.Id, out var pizzaEntry))
                        {
                            pizzaEntry = pizza;
                            pizzas.Add(pizzaEntry.Id, pizzaEntry);

                            if (orderEntry.Pizzas is null)
                            {
                                orderEntry.Pizzas = new List<OrderedPizzaWithDetails>();
                            }

                            orderEntry.Pizzas.Add(pizza);
                        }

                        // Add the found topping to the pizza it belongs to
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

        public async Task<IEnumerable<int>> GetOrdersToPhoneNumber(string phoneNumber)
        {
            return await _sqlDataAccess.LoadDataAsync<int, dynamic>("spOrder_GetIdsToPhoneNumber", new { PhoneNumber = phoneNumber });
        }

        public async Task<IEnumerable<PizzaWithMakeTime>> GetMakeTimes(string phoneNumber, DateTime orderDate)
        {
            var date = DateTimeRounder.RoundToNearestMinute(orderDate);
            _logger.LogWarning("Repo:" + date.ToString());
            var res = await _sqlDataAccess.LoadDataAsync<PizzaWithMakeTime, dynamic>(
                                                OrderRepoConstants.Sp_GetPizzasToOrderWithTime,
                                                new
                                                {
                                                    PhoneNumber = phoneNumber,
                                                    OrderDate = DateTimeRounder.RoundToNearestMinute(orderDate)
                                                });

            _logger.LogWarning("Repo count:" + res.Count());
            return res;
        }

        public async Task Create(OrderModel order, OrderedPizzaModel[] pizzas)
        {
            var orderInputs = new
            {
                order.PhoneNumber,
                OrderDate = DateTimeRounder.RoundToNearestMinute(order.OrderDate),
                order.Comment,
                order.Address.City,
                order.Address.Line1,
                order.Address.Line2
            };

            var orderId = await _sqlDataAccess.SaveWithOutput(OrderRepoConstants.Sp_CreateOrder, orderInputs);

            var orderPizzaConverted = ConvertToInputPizzas(orderId, pizzas);
            var pizzaInputs = new
            {
                pizzas = orderPizzaConverted.AsTableValuedParameter(OrderRepoConstants.OrderPizzaUdtName)
            };

            await _sqlDataAccess.SaveDataAsync(OrderRepoConstants.Sp_AttachPizzasToOrder, pizzaInputs);
        }

        public async Task CompleteOrder(string phoneNumber, DateTime orderDate)
        {
            await _sqlDataAccess.SaveDataAsync<dynamic>(
                                        OrderRepoConstants.Sp_CompleteOrder,
                                        new
                                        {
                                            PhoneNumber = phoneNumber,
                                            OrderDate = DateTimeRounder.RoundToNearestMinute(orderDate)
                                        });
        }

        private DataTable ConvertToInputPizzas(int orderId, OrderedPizzaModel[] pizzas)
        {
            var res = new DataTable();

            res.Columns.Add(OrderRepoConstants.OrderConnectorId, typeof(int));
            res.Columns.Add(OrderRepoConstants.PizzaConnectorId, typeof(int));
            res.Columns.Add(OrderRepoConstants.PizzaSizeColumnName, typeof(int));

            // Luckily foreach is nowdays as fast, or maybe even faster, as for is ;)
            foreach (var pizza in pizzas)
            {
                res.Rows.Add(orderId, pizza.Id, pizza.Size);
            }

            return res;
        }
    }
}
