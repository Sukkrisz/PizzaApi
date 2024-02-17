CREATE TABLE [dbo].[Topping]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(15) NOT NULL, 
    -- The time it takes to prepare the given topping (for example to slice the onions up)
    [PrepareTime] SMALLINT NOT NULL DEFAULT 1
)
