BEGIN

    CREATE TABLE [dbo].TT_ILogLocationCategory (
        Id INT NOT NULL PRIMARY KEY,
        Name VARCHAR(255) NOT NULL
    )

    INSERT INTO [dbo].TT_ILogLocationCategory(Id, Name) VALUES
        (0, 'OtherTTLogixManaged'),
	    (1, 'Inbound'),
        (2, 'Outbound'),
        (3, 'iLogTransfer'),
        (4, 'iLogStorage');

    ALTER TABLE [dbo].TT_Location ADD ILogLocationCategoryId INT NOT NULL
        CONSTRAINT DF_TT_Location_ILogLocationCategoryId DEFAULT 0
        CONSTRAINT FK_TT_Location_ILogLocationCategoryId FOREIGN KEY (ILogLocationCategoryId) REFERENCES TT_ILogLocationCategory(Id)

END