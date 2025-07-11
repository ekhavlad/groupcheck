CREATE TABLE [dbo].[Accounts](
	[ID] [int] IDENTITY(1,1) NOT NULL,

	[Created] [datetime2](2) NOT NULL,
	[Updated] [datetime2](2) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[Revision] [timestamp] NOT NULL,

	[Phone] [nvarchar](11) NULL,
	[Email] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[Salt] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[BlockedUntil] [datetime2](2) NULL,

	CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Accounts_Email] ON [dbo].[Accounts] ([Email] ASC) WHERE [Email] IS NOT NULL;
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Accounts_Phone] ON [dbo].[Accounts] ([Phone] ASC) WHERE [Phone] IS NOT NULL;
GO


CREATE TABLE [dbo].[Groups](
	[ID] [int] IDENTITY(1,1) NOT NULL,

	[Created] [datetime2](2) NOT NULL,
	[CreatedByID] [int] NOT NULL,
	[Updated] [datetime2](2) NOT NULL,
	[UpdatedByID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[Revision] [timestamp] NOT NULL,

	[Confirmed] [bit] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,

	CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO


CREATE TABLE [dbo].[Members](
	[GroupID] [int] NOT NULL,
	[MemberID] [int] NOT NULL,

	[Created] [datetime2](2) NOT NULL,
	[CreatedByID] [int] NOT NULL,
	[Updated] [datetime2](2) NOT NULL,
	[UpdatedByID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[Revision] [timestamp] NOT NULL,

	[Confirmed] [bit] NOT NULL,
	[AccountID] [int] NULL,
	[Name] [nvarchar](50) NOT NULL,

	CONSTRAINT [PK_Members] PRIMARY KEY CLUSTERED ([GroupID] ASC, [MemberID])
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Members_AccountID] ON [dbo].[Members] ([GroupID] ASC, [AccountID] ASC) WHERE [AccountID] IS NOT NULL;
GO


CREATE TABLE [dbo].[Checks](
	[GroupID] [int] NOT NULL,
	[CheckID] [int] NOT NULL,

	[Created] [datetime2](2) NOT NULL,
	[CreatedByID] [int] NOT NULL,
	[Updated] [datetime2](2) NOT NULL,
	[UpdatedByID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[Revision] [timestamp] NOT NULL,

	[Confirmed] [bit] NOT NULL,
	[DateAndTime] [datetimeoffset](0) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Creditors] [nvarchar](1000) NOT NULL,
	[Debitors] [nvarchar](1000) NOT NULL,

	CONSTRAINT [PK_Checks] PRIMARY KEY CLUSTERED ([GroupID] ASC, [CheckID] ASC)
)
GO


CREATE TYPE [dbo].[CreateMemberTableType] AS TABLE(
	[GroupID] [int] NOT NULL,
	[MemberID] [int] NOT NULL,
	[AccountID] [int] NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Confirmed] [bit] NOT NULL,
	[Created] [datetime2](2) NOT NULL,
	[CreatedByID] [int] NOT NULL,
	[Updated] [datetime2](2) NOT NULL,
	[UpdatedByID] [int] NOT NULL,
	[Deleted] [bit] NOT NULL
)
GO
