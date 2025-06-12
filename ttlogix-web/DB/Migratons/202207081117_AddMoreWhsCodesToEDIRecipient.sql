ALTER TABLE EDIRecipient
ADD AdditionalWHSCodes varchar(50) NULL;
GO

IF DB_NAME() = 'VMI_POLAND'
    UPDATE EDIRecipient
	SET AdditionalWHSCodes = 'WB'
	WHERE FactoryID in ('PLS','PLT','PLV','PLY')
GO