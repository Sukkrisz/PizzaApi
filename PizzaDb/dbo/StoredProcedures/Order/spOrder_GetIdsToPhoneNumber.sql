CREATE PROCEDURE [dbo].[spOrder_GetIdsToPhoneNumber]
	@PhoneNumber char(12)
AS
BEGIN
	SELECT Id
	FROM [dbo].[Order]
	WHERE [PhoneNumber] = @PhoneNumber;

	RETURN 1;
END
