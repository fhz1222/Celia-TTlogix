BEGIN
	DECLARE @OldNotStdId AS INT;

	SELECT @OldNotStdId = (SELECT TOP(1) Id FROM TT_PalletType WHERE Name = 'NOT STD');
	
	SET IDENTITY_INSERT TT_PalletType ON;
	INSERT INTO TT_PalletType(Id, Name) VALUES
		(0, 'NOT STD');
	SET IDENTITY_INSERT TT_PalletType OFF;

	UPDATE TT_PartMaster
	SET PalletTypeId = 0
	WHERE PalletTypeId = @OldNotStdId;

	DELETE FROM TT_PalletType
	WHERE Id = @OldNotStdId;

	ALTER TABLE TT_PartMaster
	ADD CONSTRAINT DF_TT_PartMaster_PalletTypeId DEFAULT(0) for PalletTypeId
END