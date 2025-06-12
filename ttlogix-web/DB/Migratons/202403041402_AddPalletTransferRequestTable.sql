BEGIN
    CREATE TABLE [dbo].TT_PalletTransferRequest (
        JobNo VARCHAR(15) NOT NULL PRIMARY KEY,
        CreatedOn DATETIME NOT NULL,
        CreatedBy VARCHAR(255) NOT NULL,
        CompletedOn DATETIME NULL, 
        PID VARCHAR(20) FOREIGN KEY REFERENCES TT_StorageDetail(PID)
    )
END