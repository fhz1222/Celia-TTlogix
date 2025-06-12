IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_PartMaster]') AND name = 'LengthTT'
)
BEGIN
    ALTER TABLE [dbo].TT_PartMaster ADD LengthTT numeric(18, 6) NOT NULL 
    DEFAULT 0.1 WITH VALUES
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_PartMaster]') AND name = 'WidthTT'
)
BEGIN
    ALTER TABLE [dbo].TT_PartMaster ADD WidthTT numeric(18, 6) NOT NULL
    DEFAULT 0.1 WITH VALUES
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_PartMaster]') AND name = 'HeightTT'
)
BEGIN
    ALTER TABLE [dbo].TT_PartMaster ADD HeightTT numeric(18, 6) NOT NULL
    DEFAULT 0.1 WITH VALUES
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_PartMaster]') AND name = 'NetWeightTT'
)
BEGIN
    ALTER TABLE [dbo].TT_PartMaster ADD NetWeightTT numeric(18, 6) NOT NULL
    DEFAULT 1 WITH VALUES
END
GO

IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_PartMaster]') AND name = 'GrossWeightTT'
)
BEGIN
    ALTER TABLE [dbo].TT_PartMaster ADD GrossWeightTT numeric(18, 6) NOT NULL
    DEFAULT 1 WITH VALUES
END
GO