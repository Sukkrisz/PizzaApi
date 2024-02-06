CREATE PROCEDURE [dbo].[spPizza_GetAll]
AS
BEGIN
	SELECT p.*, t.*
	FROM [dbo].[Pizza] p
	LEFT JOIN [dbo].[PizzaTopping] pt on pt.PizzaId = p.Id
	LEFT JOIN [dbo].[Topping] t on t.Id = pt.ToppingId;
END
