CREATE PROCEDURE [dbo].[spPizza_GetAllToOrder]
	@OrderId int
AS
BEGIN
	
	SELECT
		p.*,
		t.*
	FROM [dbo].[OrderPizza] op
	LEFT JOIN [dbo].[Pizza] p
		ON op.PizzaId = p.Id
	LEFT JOIN [dbo].[PizzaTopping] pt
		ON p.Id = pt.PizzaId
	LEFT JOIN [dbo].[Topping] t
		ON pt.ToppingId = t.Id
	WHERE op.OrderId = @OrderId;

	RETURN 1;
END
