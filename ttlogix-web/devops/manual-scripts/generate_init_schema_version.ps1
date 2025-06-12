param(
    [Parameter(Mandatory=$true)][string]$MigrationsPath
)

Write-Output @"
CREATE TABLE [dbo].[_SchemaVersions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScriptName] [nvarchar](255) NOT NULL,
	[Applied] [datetime] NOT NULL,
 CONSTRAINT [PK__SchemaVersions_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

"@

Get-ChildItem -Path $MigrationsPath -Name | ForEach-Object {$_.Trim() -replace '^(....)(..)(..)(..)(..).*$','INSERT INTO [dbo].[_SchemaVersions](ScriptName, Applied) VALUES (''$0'',''$1-$2-$3 $4:$5'')'} | Write-Output

Write-Output "GO"

