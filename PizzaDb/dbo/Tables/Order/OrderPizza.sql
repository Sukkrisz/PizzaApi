CREATE TABLE [dbo].[OrderPizza]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderId] INT NOT NULL, 
    [PizzaId] INT NOT NULL, 
    CONSTRAINT [FK_OrderPizza_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order]([Id]), 
    CONSTRAINT [FK_OrderPizza_Pizza] FOREIGN KEY ([PizzaId]) REFERENCES [dbo].[Pizza]([Id]) 
)
