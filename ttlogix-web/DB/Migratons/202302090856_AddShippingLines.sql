EXEC sp_rename 'ASNShippingLine', 'ASNShippingLine_old'
GO

EXEC sp_rename N'ASNShippingLine_old.PK_ASNShippingLine', N'PK_ASNShippingLine_old', N'INDEX';
GO

CREATE TABLE [dbo].[ASNShippingLine](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](25) NOT NULL UNIQUE,
	[Name] [nvarchar](255) NOT NULL,
	[SCAC] [varchar](4) NULL,
	[IsActive] bit NOT NULL,
	[UpdatedDate] datetime NOT NULL,
 CONSTRAINT [PK_ASNShippingLine] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeader]') AND name = 'ShippingLineID'
)
BEGIN
    ALTER TABLE [dbo].ASNHeader ADD 
	ShippingLineID INT NULL,
	CONSTRAINT FK_ASNHeader_ShippingLineID FOREIGN KEY(ShippingLineID) REFERENCES ASNShippingLine(ID);
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeaderLog]') AND name = 'ShippingLineID'
)
BEGIN
    ALTER TABLE [dbo].ASNHeaderLog ADD ShippingLineID INT NULL
END

GO