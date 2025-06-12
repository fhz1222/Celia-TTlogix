DROP TABLE dbo.[DELFORLocation];

CREATE TABLE [dbo].[DELFORLocation](
	[FactoryID] [nchar](7) NOT NULL,
	[LocationID] [nchar](25) NOT NULL,
	[LocationName] [nvarchar](100) NOT NULL,
	primary key ([FactoryID], [LocationID])
)