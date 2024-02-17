CREATE PROCEDURE [dbo].[spPizza_GetByMultiIds]
	@ids varchar(400),
	@OrderId int
AS
BEGIN

/*
	CREATE TABLE #splitted_PizzaIds (Id int);
	SELECT
		CAST(value AS int) as Id
	INTO #splitt
	FROM
		STRING_SPLIT(@ids, ';');
	*/

	CREATE TABLE #orderedPizzasWithSizes (Id int IDENTITY, PizzaId int, Size int);
	INSERT INTO #orderedPizzasWithSizes (PizzaId, Size)
	SELECT PizzaId, Size
	FROM [dbo].[OrderPizza]
	WHERE OrderId = @OrderId;

	-- Calculate the time by ordered
	SELECT op.Id, (tim.PreparationTime) + SUM(t.PrepareTime * tim.ToppingPreparationMultiplier)
	FROM #orderedPizzasWithSizes op
	LEFT JOIN [dbo].[PizzaSizePreparationTimes] tim on op.Size = tim
	LEFT JOIN [dbo].[PizzaTopping] pt ON op.PizzaId = pt.PizzaId
	LEFT JOIN [dbo].[Topping] t on pt.ToppingId = t.Id
	GROUP BY op.id;


	SELECT *
	FROM
		[dbo].[PizzaTopping]


RETURN 0
END
