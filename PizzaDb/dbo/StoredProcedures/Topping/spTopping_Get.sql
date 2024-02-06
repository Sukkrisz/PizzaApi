CREATE PROCEDURE [dbo].[spTopping_Get]
	@Id int
AS
BEGIN
	SELECT *
	FROM [dbo].[Topping]
	WHERE Id = @Id;

	RETURN 1;
END
