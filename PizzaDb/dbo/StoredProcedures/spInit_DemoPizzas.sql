CREATE PROCEDURE [dbo].[spInit_DemoPizzas]
	@ammountOfPizzasToCreate int = 1
AS
BEGIN
	DECLARE @defaultTopping1 nvarchar(15) =  'Paradicsomszósz';
	DECLARE @defaultTopping2 nvarchar(15) = 'Mozarella sajt';

	-- Insert if not present in the db yet
	EXEC [dbo].[_SpInit_DefaultToppings] @ToppingParam1 = @defaultTopping1, @ToppingParam2 = @defaultTopping2

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

	WITH TmpPizzaTopping AS(
		SELECT Id as PizzaId, @defaultToppingId1 as ToppingId
		FROM @insertedPizzaIds
		UNION
		SELECT Id as PizzaId, @defaultTopping2 as ToppingId
		FROM @insertedPizzaIds
	)

	INSERT INTO [dbo].[PizzaTopping] (PizzaId, ToppingId)
	SELECT PizzaId, ToppingId
	FROM TmpPizzaTopping
	--
	/*
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
	END;*/

	return 1;
END;