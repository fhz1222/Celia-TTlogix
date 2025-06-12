
CREATE NONCLUSTERED INDEX [idx_TT_Inbound_Status]
ON [dbo].[TT_Inbound] ([Status])
INCLUDE ([CustomerCode],[WHSCode],[IRNo],[RefNo],[ETA],[TransType],[Remark],[SupplierID])

