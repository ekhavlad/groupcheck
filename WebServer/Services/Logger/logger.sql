DROP PROCEDURE IF EXISTS [dbo].[LogRequest]
GO
DROP PROCEDURE IF EXISTS [dbo].[LogMessage]
GO
DROP INDEX IF EXISTS [IX_ResponseCodeAndTime] ON [dbo].[Requests]
GO
DROP INDEX IF EXISTS [IX_DateAndTime] ON [dbo].[Requests]
GO
DROP INDEX IF EXISTS [IX_RequestID] ON [dbo].[Logs]
GO
DROP INDEX IF EXISTS [IX_LevelAndTime] ON [dbo].[Logs]
GO
DROP TABLE IF EXISTS [dbo].[RequestsData]
GO
DROP TABLE IF EXISTS [dbo].[Requests]
GO
DROP TABLE IF EXISTS [dbo].[Logs]
GO

CREATE TABLE [dbo].[Logs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RequestID] [bigint] NOT NULL,
	[DateAndTime] [datetime2](2) NOT NULL,
	[LogLevel] [tinyint] NOT NULL,
	[Message] [nvarchar](900) NOT NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Requests](
	[ID] [bigint] NOT NULL,
	[DateAndTime] [datetime2](2) NOT NULL,
	[Method] [char](3) NOT NULL,
	[ResponseCode] [smallint] NOT NULL,
	[Duration] [int] NOT NULL,
	[Url] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Requests] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestsData](
	[RequestID] [bigint] NOT NULL,
	[Request] [nvarchar](max) NOT NULL,
	[Response] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_RequestsData] PRIMARY KEY CLUSTERED 
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE NONCLUSTERED INDEX [IX_LevelAndTime] ON [dbo].[Logs]
(
	[LogLevel] ASC,
	[DateAndTime] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RequestID] ON [dbo].[Logs]
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DateAndTime] ON [dbo].[Requests]
(
	[DateAndTime] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResponseCodeAndTime] ON [dbo].[Requests]
(
	[ResponseCode] ASC,
	[DateAndTime] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[LogMessage]
	@RequestID bigint,
	@DateAndTime datetime2(2),
	@LogLevel tinyint,
	@Message nvarchar(900)
AS
BEGIN

	SET NOCOUNT ON;
	
	INSERT INTO Logs(RequestID, DateAndTime, LogLevel, [Message])
	VALUES (@RequestID, @DateAndTime, @LogLevel, @Message)

END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[LogRequest]
	@ID bigint,
	@DateAndTime datetime2(2),
	@Method char(3),
	@ResponseCode smallint,
	@Duration int,
	@Url varchar(255),
	@Request nvarchar(1000),
	@Response nvarchar(1000)
AS
BEGIN

	SET NOCOUNT ON;
	
	INSERT INTO Requests(ID, DateAndTime, Method, ResponseCode, Duration, [Url])
	VALUES (@ID, @DateAndTime, @Method, @ResponseCode, @Duration, @Url)

	IF @Request IS NOT NULL OR @Response IS NOT NULL
	BEGIN
		INSERT INTO RequestsData(RequestID, Request, Response) VALUES (@ID, @Request, @Response)
	END

END
GO
