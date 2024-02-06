CREATE PROCEDURE [dbo].[spPizza_Insert]
	@Name nvarchar(25),
	@Price smallint
AS
BEGIN
	INSERT INTO [dbo].[Pizza]
	VALUES(@Name, @Price);

	RETURN 1;
END
