IF NOT EXISTS (
	SELECT * FROM sysobjects WHERE name = 'TT_OutboundDestinations' and xtype = 'U'
) 
BEGIN 
	CREATE TABLE [dbo].TT_OutboundDestinations (
	Id int not null primary key,
	Code varchar(255) not null,
	Name varchar(255) not null
)
END
GO

INSERT INTO [dbo].TT_OutboundDestinations VALUES (0, 'T00', 'Production Line')
GO

IF NOT EXISTS (
	SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_Outbound]') AND name = 'DestinationId'
) 
BEGIN
ALTER TABLE [dbo].TT_Outbound
ADD DestinationId int NULL
END
GO
