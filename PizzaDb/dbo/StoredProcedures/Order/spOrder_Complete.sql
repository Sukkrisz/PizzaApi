CREATE PROCEDURE [dbo].[spOrder_Complete]
	@PhoneNumber char(12),
	@OrderDate smalldatetime
AS
BEGIN
	UPDATE [dbo].[Order]
	SET [Status] = 2
	WHERE
		@PhoneNumber = @PhoneNumber AND
		OrderDate = @OrderDate;

	RETURN 1;
END
