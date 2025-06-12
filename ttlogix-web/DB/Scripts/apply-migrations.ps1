param(
    [Parameter(Mandatory=$true)][string]$ConnectionString,
    [Parameter(Mandatory=$true)][string]$ScriptPath
)

Add-Type -Path (Join-Path -Path $PSScriptRoot -ChildPath '.\dbup-core.dll')
Add-Type -Path (Join-Path -Path $PSScriptRoot -ChildPath '.\dbup-sqlserver.dll')

$dbUp = [DbUp.DeployChanges]::To
$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, $ConnectionString)
$dbUp = [StandardExtensions]::WithScriptsFromFileSystem($dbUp, $ScriptPath)
$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', '_SchemaVersions')
$dbUp = [StandardExtensions]::LogToConsole($dbUp)
$upgradeResult = $dbUp.Build().PerformUpgrade()
if ($upgradeResult.Successful -eq $false) { exit 1; }