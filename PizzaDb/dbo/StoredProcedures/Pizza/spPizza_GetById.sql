CREATE PROCEDURE [dbo].[spPizza_Get]
	@PizzaId int
AS
BEGIN
	SELECT p.*, t.*
	FROM [dbo].[Pizza] p
	LEFT JOIN [dbo].PizzaTopping pt
		ON p.Id = pt.PizzaId
	LEFT JOIN [dbo].[Topping] t
			on pt.ToppingId = t.Id
	WHERE p.Id = @PizzaId;

	RETURN 1;
END
