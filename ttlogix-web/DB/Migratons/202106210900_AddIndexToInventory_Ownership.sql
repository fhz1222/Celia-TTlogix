
CREATE NONCLUSTERED INDEX [idx_TT_Inventory_Ownership]
ON [dbo].[TT_Inventory] ([Ownership])
INCLUDE ([OnHandQty],[AllocatedQty],[QuarantineQty])

