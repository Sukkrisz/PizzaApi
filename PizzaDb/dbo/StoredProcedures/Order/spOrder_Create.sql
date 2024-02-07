CREATE PROCEDURE [dbo].[spOrder_Create]
	@PhoneNumber char(12) = 0,
	@OrderDate smallDateTime,
	@Comment nvarchar(100),
	@City nvarchar(25),
	@Line1 nvarchar(25),
	@Line2 nvarchar(15)
AS
BEGIN
	-- Set aside the inserted Id's so that they can be attached to the order object.
	CREATE TABLE #inserted_ids (Id int identity, InsertedId int)

	INSERT INTO [dbo].[Address]
	OUTPUT INSERTED.[Id] INTO #inserted_ids
	VALUES(@City, @Line1, @Line2);
	
	IF @Comment is not null
	BEGIN
		INSERT INTO [dbo].[OrderComment]
		OUTPUT INSERTED.[Id] INTO #inserted_ids
		VALUES (@Comment);
	END

	-- Address is a must have object, so it will always be present.
	-- Identity starts from 1, and is related to the addressId, since it was inserted first.
	DECLARE @addressId int;
	SELECT @addressId = InsertedId FROM #inserted_ids WHERE Id = 1;

	DECLARE @commentId int;
	SELECT @commentId = InsertedId FROM #inserted_ids WHERE Id = 2;

	INSERT INTO [dbo].[Order] ([PhoneNumber], [OrderDate], [AddressId], [CommentId])
	OUTPUT INSERTED.[Id]
	VALUES (@PhoneNumber, @OrderDate, @addressId, @commentId);

RETURN 0
END
