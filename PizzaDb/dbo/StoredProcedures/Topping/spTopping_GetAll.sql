CREATE PROCEDURE [dbo].[spTopping_GetAll]
AS
BEGIN
	SELECT * FROM [dbo].[Topping];

	RETURN 1;
END
