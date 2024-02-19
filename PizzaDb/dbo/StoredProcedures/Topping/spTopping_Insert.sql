CREATE PROCEDURE [dbo].[spTopping_Insert]
	@Name nvarchar(25),
	@Price int
AS
BEGIN
	INSERT INTO [dbo].[Topping]
	VALUES (@Name, @Price);

	RETURN 1;
END
