CREATE TABLE [dbo].[PizzaTopping]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PizzaId] INT NOT NULL, 
    [ToppingId] INT NOT NULL, 
    CONSTRAINT [FK_PizzaId] FOREIGN KEY ([PizzaId]) REFERENCES [dbo].[Pizza]([Id]),
    CONSTRAINT [FK_ToppingId] FOREIGN KEY ([ToppingId]) REFERENCES [dbo].[Topping]([Id])
)
