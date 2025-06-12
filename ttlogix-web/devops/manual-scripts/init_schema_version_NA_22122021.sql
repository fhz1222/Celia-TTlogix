CREATE TABLE [dbo].[_SchemaVersions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScriptName] [nvarchar](255) NOT NULL,
	[Applied] [datetime] NOT NULL,
 CONSTRAINT [PK__SchemaVersions_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

--INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202105131233_AddIndexToLoadingDetail_OrderNo.sql','2021-05-13 12:33')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202105171812_AddFieldsToLoading.sql','2021-05-17 18:12')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202105241110_AddXDockFieldToOutbound.sql','2021-05-24 11:10')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202105251010_AddReportPrintedFlagsToOutbound.sql','2021-05-25 10:10')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202106180000_StorageDetailGroup.sql','2021-06-18 00:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202106210900_AddIndexToInbound_Status.sql','2021-06-21 09:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202106210900_AddIndexToInventory_Ownership.sql','2021-06-21 09:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202106251200_AccessRights.sql','2021-06-25 12:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202107061000_AddDefaultSunsetPeriodToSupplierDetail.sql','2021-07-06 10:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202107061100_AddMissingColumnsToStockTransfer.sql','2021-07-06 11:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202107061200_AddNewColumnsToPartMaster.sql','2021-07-06 12:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202107061300_AddOptionalEDISyncToPartMaster.sql','2021-07-06 13:00')
--INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202109030800_UpdateAddressBook.sql','2021-09-03 08:00')
INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202109220000_StorageGroupName.sql','2021-09-22 00:00')
--INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES ('202110221600_AddDelforLocation.sql','2021-10-22 16:00')
GO

select * from _SchemaVersions