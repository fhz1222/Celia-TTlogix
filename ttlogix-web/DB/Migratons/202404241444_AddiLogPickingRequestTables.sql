CREATE TABLE [dbi].[iLogPickingRequest](
	[Id]  AS (concat('PRQ',right('0000000000'+CONVERT([nvarchar],[_Id],0),(10)))),
	[OutboundJobNo] [varchar](15) NOT NULL,
	[_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_PickingRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbi].[iLogPickingRequestRevision](
	[PickingRequestId] [varchar](15) NOT NULL,
	[Revision] [int] NOT NULL,
	[WHSCode] [varchar](7) NOT NULL,
	[CreatedBy] [varchar](10) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ClosedOn] [datetime] NULL,
 CONSTRAINT [PK_PickingRequestRevision] PRIMARY KEY CLUSTERED 
(
	[PickingRequestId] ASC, [Revision] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [dbi].[iLogPickingRequestRevisionItem](
	[PickingRequestId] [varchar](15) NOT NULL,
	[PickingRequestRevision] [int] NOT NULL,
	[LineNo] [int] NOT NULL,
	[SupplierId] [varchar](10) NOT NULL,
	[ProductCode] [varchar](30) NOT NULL,
	[Qty] [int] NOT NULL,
	[PID] [varchar](20) NULL,
 CONSTRAINT [PK_PickingRequestRevisionItem] PRIMARY KEY CLUSTERED 
(
	[PickingRequestId] ASC, [PickingRequestRevision] ASC, [LineNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

