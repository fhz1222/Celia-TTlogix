CREATE TABLE [dbo].[ContactRole](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_ContactRole] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContactInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Email] [nvarchar](320) NULL,
	[Phone] [varchar](30) NULL,
	[Mobile] [varchar](30) NULL,
	[Fax] [varchar](30) NULL,
 CONSTRAINT [PK_ContactInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SupplierContactInfo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FactoryID] [varchar](7) NOT NULL,
	[SupplierID] [varchar](10) NOT NULL,
	[Type] [varchar](1) NOT NULL,
	[ContactInfo_ID] [int] NOT NULL,
	[ContactRole_ID] [int] NULL,
 CONSTRAINT [PK_SupplierContactInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SupplierContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_SupplierContactInfo_ContactInfo] FOREIGN KEY([ContactInfo_ID])
REFERENCES [dbo].[ContactInfo] ([ID])
GO

ALTER TABLE [dbo].[SupplierContactInfo] CHECK CONSTRAINT [FK_SupplierContactInfo_ContactInfo]
GO

ALTER TABLE [dbo].[SupplierContactInfo]  WITH CHECK ADD  CONSTRAINT [FK_SupplierContactInfo_ContactRole] FOREIGN KEY([ContactRole_ID])
REFERENCES [dbo].[ContactRole] ([ID])
GO

ALTER TABLE [dbo].[SupplierContactInfo] CHECK CONSTRAINT [FK_SupplierContactInfo_ContactRole]
GO

ALTER TABLE [dbo].[SupplierContactInfo]  WITH CHECK ADD  CONSTRAINT [CK_SupplierContactInfo] CHECK  (([Type]='E' OR [Type]='V'))
GO

ALTER TABLE [dbo].[SupplierContactInfo] CHECK CONSTRAINT [CK_SupplierContactInfo]
GO

INSERT INTO ContactRole (Name) VALUES ('Buyer'), ('Planner'), ('Quality')
GO