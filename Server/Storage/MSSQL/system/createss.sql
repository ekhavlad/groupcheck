USE [master]
GO

/****** Object:  StoredProcedure [dbo].[createss]    Script Date: 22.08.2019 11:29:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[createss] 
	@Database varchar(128),
	@SnapshotName varchar(128) = null,
	@FilePath varchar(128) = 'D:\SQL\',
	@Debug bit = 0  -- print sql statement
AS
BEGIN

	DECLARE  @FileSql varchar(3000) = ''
			,@SnapSql nvarchar(4000)
				
	IF DB_ID(@Database) IS NULL
		RAISERROR('Database doesn''t exist. Please check spelling and instance you are connected to.', 1, 1)
	
	if @SnapshotName IS NULL
		SET @SnapshotName = @Database + '_ss'

	IF @FilePath IS NULL
		SET @FilePath = ''
	
	IF RIGHT(@FilePath, 1) <> '\' AND RIGHT(@FilePath, 1) <> '/'
		SET @FilePath = @FilePath + '\'
	
	SELECT @FileSql = @FileSql +
		CASE -- Case statement used to wrap a comma in the right place.
			WHEN @FileSql <> '' 
			THEN + ','
			ELSE ''
		END + '		
			( NAME = ''' + mf.name + ''', FILENAME = ''' + @FilePath + mf.name + @SnapshotName + '.ss'')'
			-- Remove file extension .mdf, .ndf, and add .ss
	FROM sys.master_files AS mf
		INNER JOIN sys.databases AS db ON db.database_id = mf.database_id
	WHERE db.state = 0 -- Only include database online.
	AND mf.type = 0 -- Only include data files.
	AND db.[name] = @Database
	
	--==================================	
	-- 3) Build the create snapshot syntax.
	--==================================
	SET @SnapSql =
	'
	CREATE DATABASE [' +@SnapshotName + ']
	    ON ' 
		+ @FileSql +
		'
	    AS SNAPSHOT OF ['+ @Database + '];'
	
	--==================================
	-- 4) Print or execute the dynamic sql.
	--==================================
	IF (@Debug = 1)
		BEGIN
			PRINT @SnapSql
		END
	ELSE
		BEGIN
			EXEC sp_executesql @stmt = @SnapSql
		END
END
GO


