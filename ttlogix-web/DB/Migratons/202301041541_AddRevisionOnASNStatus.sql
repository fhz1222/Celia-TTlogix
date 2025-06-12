IF NOT EXISTS (
  SELECT * 
  FROM sys.columns 
  WHERE object_id = OBJECT_ID(N'[dbi].[ASNHeaderStatus]') 
         AND name = 'Revision'
)
BEGIN
    ALTER TABLE [dbi].ASNHeaderStatus ADD Revision INT NOT NULL
    CONSTRAINT DF_ASNHeaderStatus_Revision DEFAULT 0
END
GO