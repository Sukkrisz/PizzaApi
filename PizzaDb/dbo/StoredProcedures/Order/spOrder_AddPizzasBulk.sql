CREATE PROCEDURE [dbo].[spOrder_AddPizzasBulk]
	@pizzas OrderPizzaUDT READONLY
AS
BEGIN
	INSERT INTO [dbo].[OrderPizza] (OrderId, PizzaId)
	SELECT [OrderId], [PizzaId]
	FROM @pizzas

	RETURN 1
END
