CREATE TABLE [dbo].[OrderPizza]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [OrderId] INT NOT NULL, 
    [PizzaId] INT NOT NULL, 
    -- 1 = Normal, 2 = Large, 3 Family
    [Size] SMALLINT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_OrderPizza_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order]([Id]), 
    CONSTRAINT [FK_OrderPizza_Pizza] FOREIGN KEY ([PizzaId]) REFERENCES [dbo].[Pizza]([Id]) 
)
