ALTER TABLE [dbo].[TT_Loading] ADD 
[TruckLicencePlate] varchar(99) NULL default NULL,
[TrailerNo] varchar(8) NULL default NULL,
[DockNo] varchar(3) NULL default NULL,
[TruckSeqNo] varchar(99) NULL default NULL,
[AllowedForDispatch] bit NOT NULL default 0,
[AllowedForDispatchModifiedDate] datetime NULL default NULL,
[AllowedForDispatchModifiedBy] varchar(10) NULL default NULL;