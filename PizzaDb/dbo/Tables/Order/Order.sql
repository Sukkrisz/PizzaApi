CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderDate] SMALLDATETIME NOT NULL, 
    [PhoneNumber] CHAR(12) NOT NULL, 
    [CommentId] INT NULL, 
    [AddressId] INT NOT NULL,
    -- 0 = pending, 1 = processing, 2 = completed, 3 = cancelled
    [Status] SMALLINT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Order_OrderComment] FOREIGN KEY ([CommentId]) REFERENCES [dbo].[OrderComment]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_Order_Address] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Address]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_Order_PhoneNumberOrderDate] ON [dbo].[Order] ([PhoneNumber], [OrderDate])
