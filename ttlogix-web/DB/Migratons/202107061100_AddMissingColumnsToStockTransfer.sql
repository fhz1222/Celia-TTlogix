
IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_StockTransfer]') AND NAME='CommInvDate')
  ALTER TABLE TT_StockTransfer ADD CommInvDate DATETIME NULL
GO

IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_StockTransfer]') AND NAME='DESADV')
  ALTER TABLE TT_StockTransfer ADD DESADV tinyint NULL
GO
