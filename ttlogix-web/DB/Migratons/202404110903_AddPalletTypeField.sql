ALTER TABLE TT_PartMaster
ADD PalletType nvarchar(10) NOT NULL
DEFAULT 'Other' WITH VALUES
CONSTRAINT TT_PartMaster_PalletType_CK
CHECK (PalletType IN ('EPAL', 'BBMS', 'Other'));