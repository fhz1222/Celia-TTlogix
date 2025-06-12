CREATE TABLE [dbo].[ASNShippingInfo](
	[ASNNo] [varchar](25) NOT NULL,
	[ATD] [datetime] NULL,
	[LatestETAtoPort] [datetime] NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ASNShippingInfo] PRIMARY KEY CLUSTERED 
(
	[ASNNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

exec sp_rename 'dbo.ASNShipmentUpdate', 'ASNShippingInfoGTNexus'
go

CREATE TABLE [dbo].[APIUpdatedShippingLines](
	[ID] [int] NOT NULL,
 CONSTRAINT [PK_APIUpdatedShippingLines] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

insert into APIUpdatedShippingLines values (59)
go

CREATE VIEW ASNShippingInfoView AS
select 
	g.ASNNo,
	g.BOL,
	g.BookingNo,
	g.ContainerSize,
	g.Status,
	g.UpdatedDate,
	g.CreatedDate,
	g.ATD,
	g.LatestETAtoPort
from ASNShippingInfoGTNexus g
join ASNHeader h on h.ASNNo=g.ASNNo
where h.ShippingLineID is null or h.ShippingLineID not in (select ID from APIUpdatedShippingLines)
union
select
	a.ASNNo,
	null as BOL,
	null as BookingNo,
	null as ContainerSize,
	null as Status,
	a.UpdatedDate,
	a.CreatedDate,
	a.ATD,
	a.LatestETAtoPort
from ASNShippingInfo a
join ASNHeader h on h.ASNNo=a.ASNNo
where h.ShippingLineID in (select ID from APIUpdatedShippingLines)
go