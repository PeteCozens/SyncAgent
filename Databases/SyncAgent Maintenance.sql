
ALTER DATABASE SyncAgent SET RECOVERY SIMPLE
GO

DBCC SHRINKFILE (SyncAgent, 1)
GO
DBCC SHRINKFILE (SyncAgent_log, 1)
GO

Declare @path nvarchar(256) 
	= 'C:\Users\peter\source\repos\SyncAgent\Databases\SyncAgent-' + replace(convert(nvarchar(100), getdate(),120),':','') + '.bak'
BACKUP DATABASE SyncAgent TO DISK = @path;
