IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeader]')
        AND name = 'LCLTrackingNo'
) BEGIN
ALTER TABLE [dbo].[ASNHeader]
ADD LCLTrackingNo varchar(30) null
END IF NOT EXISTS (
    SELECT *
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[ASNHeaderLog]')
        AND name = 'LCLTrackingNo'
) BEGIN
ALTER TABLE [dbo].[ASNHeaderLog]
ADD LCLTrackingNo varchar(30) null
END