alter table TT_StorageDetailGroup add ClosedDate DateTime null;
alter table TT_StorageDetailGroup add RepackedDate DateTime null;
GO
update TT_StorageDetailGroup set ClosedDate=GETDATE() where Status=2
update TT_StorageDetailGroup set RepackedDate=GETDATE() where Status=3
alter table TT_StorageDetailGroup drop column Status