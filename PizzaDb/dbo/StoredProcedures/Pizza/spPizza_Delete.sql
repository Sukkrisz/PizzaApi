CREATE PROCEDURE [dbo].[spPizza_Delete]
	@Id int
AS
BEGIN
	DELETE FROM [dbo].[Pizza]
	WHERE Id = @Id;

	RETURN 1;
END
