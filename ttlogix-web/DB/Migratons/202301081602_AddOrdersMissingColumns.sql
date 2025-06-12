IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DORDERSDetails]') AND name = 'LocationID'
)
BEGIN
    ALTER TABLE [dbo].DORDERSDetails ADD LocationID VARCHAR(25) NULL
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DORDERSDetails]') AND name = 'TolerancePercentage'
)
BEGIN
    ALTER TABLE [dbo].DORDERSDetails ADD TolerancePercentage DECIMAL(18, 3) NULL
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[DORDERSDetails]') AND name = 'UnlimitedFlag'
)
BEGIN
    ALTER TABLE [dbo].DORDERSDetails ADD UnlimitedFlag TINYINT NULL
END
GO