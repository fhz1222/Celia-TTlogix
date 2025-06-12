ALTER TABLE TT_Outbound
ADD FOREIGN KEY (DestinationId) REFERENCES TT_OutboundDestinations(Id);