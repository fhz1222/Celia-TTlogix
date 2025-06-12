BEGIN 
	CREATE TABLE TT_PalletType (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		Name VARCHAR(255) NOT NULL
	);

	ALTER TABLE TT_PartMaster
	ADD PalletTypeId INT NULL
	CONSTRAINT FK_TT_PartMaster_PalletTypeId FOREIGN KEY(PalletTypeId) REFERENCES TT_PalletType(Id)

	INSERT INTO TT_PalletType(Name) VALUES
		('BBMS BOX'),
		('BBMS MAGNUM'),
		('BBMS OPTIMUM'),
		('1200x800 - AGV'),
		('1200x800 - BOX Out'),
		('1200x800 - not AGV'),
		('NOT STD');
END

GO

BEGIN
	DECLARE @BBMSBOXId AS INT;
	DECLARE @AGVId AS INT;
	DECLARE @NotAGVId AS INT;

	SELECT @BBMSBOXId = (SELECT TOP(1) Id FROM TT_PalletType WHERE Name = 'BBMS BOX');
	SELECT @AGVId = (SELECT TOP(1) Id FROM TT_PalletType WHERE Name = '1200x800 - AGV');
	SELECT @NotAGVId = (SELECT TOP(1) Id FROM TT_PalletType WHERE Name = '1200x800 - not AGV');
	
	UPDATE TT_PartMaster
	SET PalletTypeId = (CASE 
		WHEN PalletType = 'BBMS' THEN @BBMSBOXId
		WHEN PalletType = 'EPAL' THEN @AGVId
		WHEN PalletType = 'Other' THEN @NotAGVId
	END);
END

BEGIN
	DECLARE @DFPalletTypeConstraintName AS nvarchar(128);

	ALTER TABLE TT_PartMaster
	ALTER COLUMN PalletTypeId INT NOT NULL;
	
	SELECT @DFPalletTypeConstraintName = (SELECT df.name
		FROM sys.default_constraints df
		INNER JOIN sys.tables t ON df.parent_object_id = t.object_id
		INNER JOIN sys.all_columns c ON df.parent_object_id = c.object_id and df.parent_column_id = c.column_id
		WHERE t.name = 'TT_PartMaster' AND c.name = 'PalletType');

	EXEC('ALTER TABLE TT_PartMaster DROP CONSTRAINT ' + @DFPalletTypeConstraintName)

	ALTER TABLE TT_PartMaster
	DROP CONSTRAINT TT_PartMaster_PalletType_CK;

	ALTER TABLE TT_PartMaster
	DROP COLUMN PalletType;
END
