CREATE TABLE [dbo].[Order]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderDate] SMALLDATETIME NOT NULL, 
    [PhoneNumber] CHAR(12) NOT NULL, 
    [CommentId] INT NULL, 
    [AddressId] INT NOT NULL, 
    CONSTRAINT [FK_Order_OrderComment] FOREIGN KEY ([CommentId]) REFERENCES [dbo].[OrderComment]([Id]), 
    CONSTRAINT [FK_Order_Address] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Address]([Id])
)

GO

CREATE INDEX [IX_Order_PhoneNumber] ON [dbo].[Order] ([PhoneNumber])
