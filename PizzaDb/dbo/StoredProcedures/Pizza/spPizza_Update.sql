CREATE PROCEDURE [dbo].[spPizza_Update]
	@Id int,
	@Name nvarchar(25),
	@Price smallint
AS
BEGIN
	UPDATE [dbo].[Pizza]
	SET [Name] = @Name, [Price] = @Price
	WHERE Id = @Id;

	RETURN 1;
END
