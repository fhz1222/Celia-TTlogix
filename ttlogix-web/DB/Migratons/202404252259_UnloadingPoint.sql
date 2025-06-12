BEGIN
	DECLARE @PartMasterCode AS VARCHAR(15);
	
    CREATE TABLE TT_UnloadingPoint (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name VARCHAR(255) NOT NULL,
        CustomerCode VARCHAR(10) NOT NULL
    );

    CREATE TABLE TT_UnloadingPointDefault (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerCode VARCHAR(10) NOT NULL,
        SupplierID VARCHAR(10) NOT NULL,
        DefaultUnloadingPointId INT NOT NULL,
        CONSTRAINT FK_TT_UnloadingPointDefault_DefaultUnloadingPointId FOREIGN KEY(DefaultUnloadingPointId) REFERENCES TT_UnloadingPoint(Id),
        CONSTRAINT UC_TT_UnloadingPointDefault_CustomerCodeSupplierID UNIQUE(CustomerCode, SupplierID)
    );

	ALTER TABLE TT_PartMaster
	ADD UnloadingPointId INT NULL
    CONSTRAINT FK_TT_PartMaster_UnloadingPointId FOREIGN KEY(UnloadingPointId) REFERENCES TT_UnloadingPoint(Id);
		
	SELECT @PartMasterCode = (SELECT TOP(1) Code FROM TT_SystemModule WHERE ModuleName = 'PARTMASTER');
	INSERT INTO TT_SystemModule VALUES ('MDL090500000000', @PartMasterCode, 'PARTMASTERUNLOADING', 'UNLOADING');

END