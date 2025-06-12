IF NOT EXISTS (
  SELECT * 
  FROM sys.columns 
  WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeader]') 
         AND name = 'CarrierTransportNo'
)
BEGIN
    ALTER TABLE [dbo].ASNHeader ADD CarrierTransportNo VARCHAR(30) NULL
END
GO


IF NOT EXISTS (
  SELECT * 
  FROM sys.columns 
  WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeaderLog]') 
         AND name = 'CarrierTransportNo'
)
BEGIN
    ALTER TABLE [dbo].ASNHeaderLog ADD CarrierTransportNo VARCHAR(30) NULL
END
GO
