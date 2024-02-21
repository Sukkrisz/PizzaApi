CREATE PROCEDURE [dbo].[spInit_DefaultToppings]
	@ToppingParam1 nvarchar(15),
	@ToppingParam2 nvarchar(15)
AS
BEGIN
	-- Default topping 1
	IF NOT EXISTS(SELECT 1 FROM [dbo].[Topping] WHERE [Name] = @ToppingParam1)
	BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [PrepareTime])
		VALUES (@ToppingParam1, 15);
	END

	-- Default topping 2
	IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[Topping] WHERE [Name] = @ToppingParam2)
	BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [PrepareTime])
		VALUES (@ToppingParam2, 20);
	END

	-- A string split could be used to make this methdod generic and be able to send in any number of values.
	/*CREATE TABLE #splitted_PizzaIds (Id int);
	SELECT
		CAST(value AS int) as Id
	INTO #splitt
	FROM
		STRING_SPLIT(@toppingNames, ';');*/


	RETURN 1;
END
