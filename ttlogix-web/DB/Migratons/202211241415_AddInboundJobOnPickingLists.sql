IF NOT EXISTS (
  SELECT * 
  FROM sys.columns 
  WHERE object_id = OBJECT_ID(N'[dbo].[TT_PickingList]') 
         AND name = 'InboundJobNo'
)
BEGIN
    ALTER TABLE [dbo].TT_PickingList ADD InboundJobNo VARCHAR(15) NOT NULL
    CONSTRAINT DF_TT_PickingList_InboundJobNo DEFAULT ''
END
GO