CREATE PROCEDURE [Lobby].[AddPlayerToGame] 
	@GameId INT,
	@UserId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(SELECT 1 FROM Players WHERE GameId = @GameId AND UserId = @UserId)
		THROW 50409, 'Duplicate player.', 1

	IF (SELECT COUNT(1) FROM Players WHERE GameId = @GameId) 
	 = (SELECT BoardRegionCount FROM Games WHERE GameId = @GameId)
		THROW 50400, 'Max player count reached.', 1
                      
	IF (SELECT GameStatusId FROM Games WHERE GameId = @GameId) <> 1
		THROW 50400, 'Game no longer open.', 1

	INSERT INTO Players (
		GameId, 
		UserId, 
		[Name])
	SELECT 
		@GameId, 
		UserId, 
		[Name]
	FROM Users 
	WHERE UserId = @UserId
	
	SELECT SCOPE_IDENTITY()
END