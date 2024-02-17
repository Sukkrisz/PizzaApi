CREATE PROCEDURE [dbo].[spOrder_AddPizzasBulk]
	@pizzas OrderPizzaUDT READONLY
AS
BEGIN
	INSERT INTO [dbo].[OrderPizza] (OrderId, PizzaId, Size)
	SELECT [OrderId], [PizzaId], [Size]
	FROM @pizzas

	RETURN 1
END
