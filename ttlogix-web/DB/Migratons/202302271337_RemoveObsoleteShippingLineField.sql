IF EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeader]') AND name = 'ShippingLineEU'
)
BEGIN
	ALTER TABLE ASNHeader
	DROP COLUMN ShippingLineEU;
END
GO