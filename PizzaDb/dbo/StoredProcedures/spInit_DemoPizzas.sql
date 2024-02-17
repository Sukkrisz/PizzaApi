CREATE PROCEDURE [dbo].[spInit_DemoPizzas]
	@ammountOfPizzasToCreate int = 1
AS
BEGIN
	DECLARE @defaultTopping1 nvarchar(15) =  'Paradicsomszósz';
	DECLARE @defaultTopping2 nvarchar(15) = 'Mozarella sajt';

	-- Default topping 1
	IF NOT EXISTS(SELECT 1 FROM [dbo].[Topping] WHERE [Name] = @defaultTopping1)
	BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [PrepareTime])
		VALUES (@defaultTopping1, 15);
	END

	-- Default topping 2
	IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping2)
	BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [PrepareTime])
		VALUES (@defaultTopping2, 20);
	END

	-- Set aside their Id's so, that we can connect the two later on with the pizzas
	DECLARE @defaultToppingId1 int;
	SELECT @defaultToppingId1 = Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping1;

	DECLARE @defaultToppingId2 int;
	SELECT @defaultToppingId2 = Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping2;

	-- Get the max identity, so it can be used for the naming of the newly inserted pizzas
	DECLARE @maxIdentity int = IDENT_CURRENT('dbo.Pizza');
	SET @maxIdentity = ISNULL(@maxIdentity, 0);

	-- First the pizzas will be generated into a table variable, then the whole will be inserted  together from a select
	-- Not sure which performs better this, simply inserting into the table in the loop or into tmp table and from there into the db table
	-- Would need testing
	DECLARE @tmpPizzas TABLE ([Name] nvarchar(15), [Price] smallint);
	DECLARE @insertedPizzaIds TABLE (Id int);

	DECLARE @i int = 1;
	WHILE(@i <= @ammountOfPizzasToCreate)
	BEGIN
		SET @maxIdentity = @maxIdentity + 1;

		INSERT INTO @tmpPizzas ([Name], [Price])
		VALUES ('Pizza_' + CAST(@maxIdentity AS nvarchar(9)), 1500);

		SET @i = @i + 1;
	END

	INSERT INTO [dbo].[Pizza]
	OUTPUT inserted.Id INTO @insertedPizzaIds
	SELECT * FROM @tmpPizzas;

	--
	DECLARE @cursor CURSOR;
	DECLARE @currentPizzaId int;
	BEGIN
		SET @cursor = CURSOR FOR
		select Id FROM @insertedPizzaIds;

		OPEN @cursor
		FETCH NEXT FROM @cursor
		INTO @currentPizzaId;

		WHILE @@FETCH_STATUS = 0
		BEGIN
			INSERT INTO [dbo].[PizzaTopping] ([PizzaId], [ToppingId])
			VALUES
				(@currentPizzaId, @defaultToppingId1),
				(@currentPizzaId, @defaultToppingId2);

			FETCH NEXT FROM @cursor
			INTO @currentPizzaId;
		END;

		CLOSE @cursor;
		DEALLOCATE @cursor;
	END;

	return 1;
END;