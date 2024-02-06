CREATE PROCEDURE [dbo].[spCreate_DemoPizzas]
	@lastCreatedId int output,
	@ammountOfPizzasToCreate int = 1
AS
BEGIN
	DECLARE @defaultTopping1 nvarchar(15) =  'Paradicsomszósz';
	DECLARE @defaultTopping2 nvarchar(15) = 'Mozarella sajt';

	DECLARE @name nvarchar(25);
	DECLARE @i int = 1;
	DECLARE @previousMaxIdentity int = IDENT_CURRENT('dbo.Pizza');
	SET @previousMaxIdentity = ISNULL(@previousMaxIdentity, 1);

	-- Default topping 1
	IF NOT EXISTS(SELECT 1 FROM [dbo].[Topping] WHERE [Name] = @defaultTopping1) BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [Price])
		VALUES (@defaultTopping1, 0);
	END

	-- Default topping 2
	IF NOT EXISTS(SELECT TOP 1 Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping2) BEGIN
		INSERT INTO [dbo].[Topping] ([Name], [Price])
		VALUES (@defaultTopping2, 0);
	END

	DECLARE @defaultToppingId1 int;
	SELECT @defaultToppingId1 = Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping1;

	DECLARE @defaultToppingId2 int;
	SELECT @defaultToppingId2 = Id FROM [dbo].[Topping] WHERE [Name] = @defaultTopping2;

	WHILE(@i <= @ammountOfPizzasToCreate)
	BEGIN
		SET @lastCreatedId = @previousMaxIdentity + @i;

		INSERT INTO [dbo].[Pizza] ([Name], [Price])
		VALUES ('Pizza_' + CAST(@lastCreatedId AS nvarchar(9)), 1500)

		INSERT INTO [dbo].[PizzaTopping] ([PizzaId], [ToppingId])
		VALUES
			(@lastCreatedId, @defaultToppingId1),
			(@lastCreatedId, @defaultToppingId2);
		
		SET @i = @i + 1;
	END

	SET @lastCreatedId = @previousMaxIdentity + @i;
	return 1;
END