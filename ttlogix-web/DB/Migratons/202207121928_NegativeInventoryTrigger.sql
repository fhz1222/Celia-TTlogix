CREATE TRIGGER negative_inventory ON TT_Inventory
AFTER INSERT, UPDATE as
   IF EXISTS (SELECT * FROM inserted WHERE AllocatedQty < 0)
   BEGIN
        DECLARE @sql nvarchar(max)
        SET @sql = 'DBCC INPUTBUFFER(' + CAST(@@SPID AS nvarchar(100)) + ')'
        CREATE TABLE #SQL (
            EventType varchar(100),
            Parameters int,
            EventInfo nvarchar(max)
        );
        INSERT INTO #SQL EXEC sp_executesql @sql
        SELECT @sql = EventInfo FROM #SQL
        DROP TABLE #SQL
        insert into logs values(@sql, APP_NAME(), HOST_NAME())
   END
GO