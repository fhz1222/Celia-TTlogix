ALTER TABLE TT_StorageDetail ADD [GroupID] varchar(20) default null;
CREATE NONCLUSTERED INDEX [TT_StorageDetail]
ON [dbo].[TT_StorageDetail] ([GroupID])
INCLUDE ([PID]);
CREATE TABLE dbo.[TT_StorageDetailGroup]  
   (GroupID varchar(20) not null PRIMARY KEY,
   CreatedDate DATETIME NOT NULL,
   Status INT NOT NULL,
   Quantity INT DEFAULT 0);


