CREATE PROCEDURE [dbo].[spTopping_Update]
	@Id int = 0,
	@Name nvarchar(15),
	@PrepareTime smallint
AS
BEGIN
	UPDATE [dbo].[Topping]
	SET [Name] = @Name, [PrepareTime] = @PrepareTime
	WHERE Id = @Id;

	RETURN 1;
END
