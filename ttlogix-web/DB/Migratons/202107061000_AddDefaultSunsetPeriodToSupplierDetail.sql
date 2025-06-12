IF NOT EXISTS (SELECT * FROM syscolumns
  WHERE ID=OBJECT_ID('[dbo].[SupplierDetail]') AND NAME='DefaultSunsetPeriod')
  ALTER TABLE SupplierDetail ADD DefaultSunsetPeriod INT NULL
