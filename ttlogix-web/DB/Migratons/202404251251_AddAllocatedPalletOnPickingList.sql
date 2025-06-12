IF NOT EXISTS (
  SELECT * 
  FROM sys.columns 
  WHERE object_id = OBJECT_ID(N'[dbo].[TT_PickingList]') 
         AND name = 'AllocatedPID'
)
BEGIN
    ALTER TABLE [dbo].TT_PickingList ADD AllocatedPID VARCHAR(20) NULL
END
GO