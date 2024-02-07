CREATE PROCEDURE [dbo].[spOrder_GetWithPizzasEF]
	@OrderId int
AS
BEGIN
	SELECT o.Id, o.PhoneNumber, o.OrderDate, c.Comment, a.*, p.*, t.*
	FROM [dbo].[Order] o
		LEFT JOIN [dbo].[OrderComment] c on c.Id = o.CommentId
		LEFT JOIN [dbo].[Address] a on a.Id = o.AddressId
		JOIN [dbo].[OrderPizza] op on op.OrderId = o.Id
		JOIN [dbo].[Pizza] p on p.Id = op.PizzaId
			LEFT JOIN [dbo].[PizzaTopping] pt on pt.PizzaId = p.Id
			LEFT JOIN [dbo].[Topping] t on t.Id = pt.ToppingId
	WHERE o.Id = @OrderId;

RETURN 0
END
