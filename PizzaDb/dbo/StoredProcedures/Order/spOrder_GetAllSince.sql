CREATE PROCEDURE [dbo].[spOrder_GetAllSince]
	@since smalldatetime
AS
BEGIN
	SELECT o.Id, o.OrderDate, o.PhoneNumber, c.Comment, a.* 
	FROM [dbo].[Order] o
	LEFT JOIN [dbo].[Address] a on a.Id = o.AddressId
	LEFT JOIN [dbo].[OrderComment] c on c.Id = o.CommentId
	WHERE o.OrderDate >= @since
	ORDER BY o.OrderDate DESC;

	RETURN 1;
END
