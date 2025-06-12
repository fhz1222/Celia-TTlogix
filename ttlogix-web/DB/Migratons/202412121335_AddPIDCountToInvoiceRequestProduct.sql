ALTER TABLE [dbo].[InvoiceRequestProduct] ADD [PIDCount] INT NULL
GO
UPDATE InvoiceRequestProduct SET PIDCount = 0
GO
ALTER TABLE [dbo].[InvoiceRequestProduct] ALTER COLUMN [PIDCount] INT NOT NULL
