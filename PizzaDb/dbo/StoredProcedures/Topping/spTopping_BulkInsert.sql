CREATE PROCEDURE [dbo].[spTopping_BulkInsert]
	@toppings ToppingUDT READONLY
AS
BEGIN

	CREATE TABLE #inserted_ids (InsertedId int);

	INSERT INTO [dbo].[Topping] ([Name], PrepareTime)
	OUTPUT INSERTED.[Id] INTO #inserted_ids
	SELECT [Name], PrepareTime
	FROM @toppings;

	-- Return the number of items inserted
	SELECT COUNT(1) FROM #inserted_ids;

	RETURN 0
END
