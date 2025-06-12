IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestEmailCustomsAgency' and xtype='U')
CREATE TABLE [dbo].[InvoiceRequestEmailCustomsAgency](
	[Email] [nvarchar](256) UNIQUE NOT NULL
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestEmailToyotaPlanner' and xtype='U')
CREATE TABLE [dbo].[InvoiceRequestEmailToyotaPlanner](
	[Email] [nvarchar](256) UNIQUE NOT NULL
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InvoiceRequestEmailElectrolux' and xtype='U')
BEGIN
	CREATE TABLE [dbo].[InvoiceRequestEmailElectrolux](
		[Email] [nvarchar](256) NOT NULL,
		[FactoryID] [varchar](7) NULL
	);
	ALTER TABLE InvoiceRequestEmailElectrolux ADD CONSTRAINT UC_InvoiceRequestEmailElectrolux UNIQUE (Email, FactoryID);
END