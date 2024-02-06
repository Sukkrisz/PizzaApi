CREATE PROCEDURE [dbo].[spPizza_GetList]
	@page int = 1,
	@pageSize int = 5
AS
BEGIN
	DECLARE @numberOfItemsToTake int = (@page * @pageSize) + 1;
	DECLARE @itemsToSkip int = 0;
	
	IF @page != 1
		SET @itemsToSkip = (@page - 1) * @pageSize;

	
	-- Get the paged result in the subquery, and then connect the toppings related to the pizzas
	SELECT sq.*, t.*
	FROM
		(SELECT *
		FROM [dbo].[Pizza]
		ORDER BY Id
		OFFSET @itemsToSkip ROWS
		FETCH NEXT @numberOfItemsToTake ROWS ONLY) AS sq
	LEFT JOIN [dbo].[PizzaTopping] pt on pt.PizzaId = sq.Id
	LEFT JOIN [dbo].[Topping] t on t.Id = pt.ToppingId;
	

	RETURN 1
END
