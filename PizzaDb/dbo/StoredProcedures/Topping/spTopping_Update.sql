CREATE PROCEDURE [dbo].[spTopping_Update]
	@Id int = 0,
	@Name nvarchar(15),
	@Price smallint
AS
BEGIN
	UPDATE [dbo].[Topping]
	SET [Name] = @Name, [Price] = @Price
	WHERE Id = @Id;

	RETURN 1;
END
