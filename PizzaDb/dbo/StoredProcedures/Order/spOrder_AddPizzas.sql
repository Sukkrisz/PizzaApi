CREATE PROCEDURE [dbo].[spOrder_AddPizzas]
	@pizzas OrderPizzaUDT READONLY
AS
BEGIN
	INSERT INTO [dbo].[OrderPizza] (OrderId, PizzaId)
	SELECT [OrderId], [PizzaId]
	FROM @pizzas

	RETURN 1
END
