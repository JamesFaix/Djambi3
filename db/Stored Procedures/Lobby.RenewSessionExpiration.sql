﻿CREATE PROCEDURE [Lobby].[RenewSessionExpiration]
	@SessionId INT,
	@ExpiresOn DATETIME2
AS
BEGIN		
	IF NOT EXISTS(SELECT 1 FROM Sessions WHERE SessionId = @SessionId)
		THROW 50000, 'Session not found', 1

	UPDATE [Sessions]
	SET ExpiresOn = @ExpiresOn
	WHERE SessionId = @SessionId
END