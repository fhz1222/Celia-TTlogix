
IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_PartMaster]') AND NAME='MasterSlave')
  ALTER TABLE TT_PartMaster ADD MasterSlave bit NOT NULL DEFAULT 1
GO

IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_PartMaster]') AND NAME='BoxItem')
  ALTER TABLE TT_PartMaster ADD BoxItem bit NOT NULL DEFAULT 0
GO

IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_PartMaster]') AND NAME='FloorStackability')
  ALTER TABLE TT_PartMaster ADD FloorStackability int NOT NULL DEFAULT 1
GO

IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_PartMaster]') AND NAME='TruckStackability')
  ALTER TABLE TT_PartMaster ADD TruckStackability int NOT NULL DEFAULT 1
GO

IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_PartMaster]') AND NAME='BoxesInPallet')
  ALTER TABLE TT_PartMaster ADD BoxesInPallet int NOT NULL DEFAULT 1
GO
