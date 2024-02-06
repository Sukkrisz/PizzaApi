CREATE PROCEDURE [dbo].[spTopping_Delete]
	@Id int
AS
BEGIN
	DELETE FROM [dbo].[Topping] WHERE Id = @Id;

	RETURN 1;
END
