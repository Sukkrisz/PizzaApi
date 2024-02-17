using Dapper;
using Data.Db.DbAccess;
using Data.Db.Helpers;
using Data.Db.Models.Order;
using Data.Db.Models.Pizza;
using Data.Db.Repositories;
using Data.Db.Repositories.Interfaces;
using Infrastructure.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models.Db;
using Models.Shared.Func;
using System.Data;

namespace Core.Data.Repositories
{
    public class OrderRepo : BaseRepo, IOrderRepo
    {
        private readonly IOptions<ConnectionStringSettings> _settings;
        private readonly ILogger _logger;

        public OrderRepo(IOptions<ConnectionStringSettings> settings, ISqlDataAccess sqlDataAccess, ILogger<OrderRepo> logger) : base(sqlDataAccess)
        {
            _settings = settings;
            _logger = logger;
        }

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
                    var pizzas = await _sqlDataAccess.LoadDataMultiObjectAsync<PizzaModel, ToppingModel, dynamic>(
                                                                            OrderRepoConstants.Sp_GetPizzasToOrder,
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
                    sql: OrderRepoConstants.Sp_GetWithPizzasEf,
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
                                                new { PhoneNumber = phoneNumber, OrderDate = DateTimeRounder.RoundToNearestMinute(orderDate) });

            _logger.LogWarning("Repo count:" + res.Count());
            return res;
        }

        public async Task Create(OrderModel order, OrderedPizzaModel[] pizzas)
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

            var orderId = await _sqlDataAccess.SaveWithOutput(OrderRepoConstants.Sp_CreateOrder, orderInputs);

            var orderPizzaConverted = ConvertToInputPizzas(orderId, pizzas);
            var pizzaInputs = new
            {
                pizzas = orderPizzaConverted.AsTableValuedParameter(OrderRepoConstants.OrderPizzaUdtName)
            };

            await _sqlDataAccess.SaveDataAsync(OrderRepoConstants.Sp_AttachPizzasToOrder, pizzaInputs);
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
