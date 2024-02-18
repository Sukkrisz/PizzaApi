CREATE PROCEDURE [dbo].[spOrder_GetMakeTimesByPhoneNumber]
	@PhoneNumber char(12),
	@OrderDate smalldatetime
AS
BEGIN

	CREATE TABLE #orderedPizzasWithSizes (Id int IDENTITY, PizzaId int, Size int);
	INSERT INTO #orderedPizzasWithSizes (PizzaId, Size)
	SELECT PizzaId, Size
	FROM [dbo].[OrderPizza] op
	JOIN [dbo].[Order] o on op.OrderId = o.Id
	WHERE o.PhoneNumber = @PhoneNumber AND  o.OrderDate = @OrderDate;

	-- Calculate the times it takes to prepare the toppings by the ordered pizzas
	WITH cte_toppingPrepareTimes (id, preparationTime) AS
	(SELECT op.Id, SUM(t.PrepareTime * tim.ToppingPreparationMultiplier)
	FROM #orderedPizzasWithSizes op
	LEFT JOIN [dbo].[PizzaSizePreparationTimes] tim on op.Size = tim.Size
	LEFT JOIN [dbo].[PizzaTopping] pt ON op.PizzaId = pt.PizzaId
	LEFT JOIN [dbo].[Topping] t on pt.ToppingId = t.Id
	GROUP BY op.Id)

	-- Add the default preparation time (according to size) to the calculated topping times
	SELECT op.PizzaId as PizzaId, op.Size as Size, pt.PreparationTime + tt.preparationTime as TimeToMake
	FROM
	#orderedPizzasWithSizes op
	LEFT JOIN [dbo].[PizzaSizePreparationTimes] pt on op.Size = pt.Size
	LEFT JOIN cte_toppingPrepareTimes tt ON op.Id = tt.id;

	RETURN 1;
END
