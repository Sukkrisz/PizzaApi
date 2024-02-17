CREATE PROCEDURE [dbo].[spInit_SizeMultipliers]
AS
BEGIN
	DECLARE @prepTimesCount int;
	SELECT @prepTimesCount = COUNT(1) FROM [dbo].[PizzaSizePreparationTimes] 

	IF @prepTimesCount != 3
	BEGIN
		INSERT INTO [dbo].[PizzaSizePreparationTimes] (Size, PreparationTime, ToppingPreparationMultiplier)
		VALUES
		(1, 120, 1),
		(2, 150, 1.2),
		(3, 200, 1.5)
	END;

	RETURN 0
END
