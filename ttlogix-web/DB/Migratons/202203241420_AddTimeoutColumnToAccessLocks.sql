IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[TT_AccessLock]') AND NAME='Timeout')
  ALTER TABLE TT_AccessLock ADD Timeout int NULL
GO