CREATE PROCEDURE [dbo].[spOrder_Get]
	@OrderId int
AS
BEGIN
	SELECT o.Id, o.OrderDate, o.PhoneNumber, c.Comment, a.*
	FROM [dbo].[Order] o
		LEFT JOIN [dbo].[OrderComment] c on c.Id = o.CommentId
		LEFT JOIN [dbo].[Address] a on a.Id = o.AddressId
	WHERE o.Id = @OrderId
	RETURN 0
END
