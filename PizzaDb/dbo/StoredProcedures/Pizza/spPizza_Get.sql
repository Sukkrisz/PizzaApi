CREATE PROCEDURE [dbo].[spPizza_Get]
	@Id int
AS
BEGIN
	SELECT *
	FROM [dbo].[Pizza]
	WHERE Id = @Id;

	RETURN 1;
END
