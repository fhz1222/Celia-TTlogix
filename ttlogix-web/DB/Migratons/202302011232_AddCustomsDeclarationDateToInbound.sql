IF NOT EXISTS (
  SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TT_Inbound]') AND name = 'CustomsDeclarationDate'
)
BEGIN
    ALTER TABLE [dbo].TT_Inbound ADD CustomsDeclarationDate DATETIME NULL
    CONSTRAINT DF_TT_Inbound_CustomsDeclarationDate DEFAULT NULL
END
GO