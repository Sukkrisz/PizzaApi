CREATE TABLE [dbo].[PizzaSizePreparationTimes]
(
    -- No Id needed. Site can be used as Id and since it will only have very few rows, it can stay as a heap
    [Size] SMALLINT NOT NULL, 
    [PreparationTime] SMALLINT NOT NULL, 
    [ToppingPreparationMultiplier] DECIMAL(2, 1) NOT NULL
)
