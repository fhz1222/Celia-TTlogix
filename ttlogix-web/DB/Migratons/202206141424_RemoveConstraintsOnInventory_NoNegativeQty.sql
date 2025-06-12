IF (OBJECT_ID('dbo.no_negative_allocated_qty', 'C') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[TT_Inventory] DROP CONSTRAINT [no_negative_allocated_qty]
END
IF (OBJECT_ID('dbo.no_negative_discrepancy_qty', 'C') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[TT_Inventory] DROP CONSTRAINT no_negative_discrepancy_qty
END
IF (OBJECT_ID('dbo.no_negative_onhand_qty', 'C') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[TT_Inventory] DROP CONSTRAINT no_negative_onhand_qty
END
IF (OBJECT_ID('dbo.no_negative_quarantine_qty', 'C') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[TT_Inventory] DROP CONSTRAINT no_negative_quarantine_qty
END
IF (OBJECT_ID('dbo.no_negative_transit_qty', 'C') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[TT_Inventory] DROP CONSTRAINT no_negative_transit_qty
END
