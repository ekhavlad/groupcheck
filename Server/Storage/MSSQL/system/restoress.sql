USE [master]
GO

/****** Object:  StoredProcedure [dbo].[restoress]    Script Date: 22.08.2019 11:29:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[restoress]
	@Database varchar(128),
	@Snapshot varchar(128) = null

AS
BEGIN

	IF (@Snapshot IS NULL)
		SET @Snapshot = @Database + '_ss'

	DECLARE @kill varchar(8000) = '';
	SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';' 
		FROM sys.dm_exec_sessions
		WHERE database_id  = db_id(@Database)

	EXEC(@kill);

	RESTORE DATABASE @Database from   
	DATABASE_SNAPSHOT = @Snapshot;  

END
GO


