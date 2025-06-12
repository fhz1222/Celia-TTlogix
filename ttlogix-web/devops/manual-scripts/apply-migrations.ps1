param(
    [Parameter(Mandatory=$true)][string]$DBServer,
    [Parameter(Mandatory=$true)][string]$DBName,
    [Parameter(Mandatory=$true)][string]$User,
    [Parameter(Mandatory=$true)][securestring]$Password,
    [Parameter(Mandatory=$true)][string]$ScriptPath
)

Add-Type -Path (Join-Path -Path $PSScriptRoot -ChildPath '.\dbup-core.dll')
Add-Type -Path (Join-Path -Path $PSScriptRoot -ChildPath '.\dbup-sqlserver.dll')

$Ptr = [System.Runtime.InteropServices.Marshal]::SecureStringToCoTaskMemUnicode($Password)
$result = [System.Runtime.InteropServices.Marshal]::PtrToStringUni($Ptr)
[System.Runtime.InteropServices.Marshal]::ZeroFreeCoTaskMemUnicode($Ptr)

$dbUp = [DbUp.DeployChanges]::To
$dbUp = [SqlServerExtensions]::SqlDatabase($dbUp, "server=$DBServer;database=$DBName;user id=$User;password=$result;Trusted_Connection=Yes;Integrated Security=false;Connection Timeout=15;")
$dbUp = [StandardExtensions]::WithScriptsFromFileSystem($dbUp, $scriptPath)
$dbUp = [SqlServerExtensions]::JournalToSqlTable($dbUp, 'dbo', '_SchemaVersions')
$dbUp = [StandardExtensions]::LogToConsole($dbUp)
$res = $dbUp.Build().PerformUpgrade()
if ($res.Successful -eq $false) { exit 1; }