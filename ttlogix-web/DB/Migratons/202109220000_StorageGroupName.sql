
IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_StockDetailGroup]') AND NAME='Name')
  ALTER TABLE TT_StorageDetailGroup ADD Name varchar(255);
  ALTER TABLE TT_StorageDetailGroup ADD WHSCode varchar(7);
GO
    update TT_StorageDetailGroup set whsCode = 'WB', Name = 'Group';
GO

