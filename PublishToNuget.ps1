[CmdletBinding()]
param (   
    [Parameter(Mandatory=$True)]
    [string]$apikey
)
$ErrorActionPreference = 'Stop'
$file = gci -recurse -filter "*.nupkg" -File -Path dist | sort -property LastWriteTime | Select-Object -Last 1
nuget push $file.FullName -Source "https://www.nuget.org/api/v2/package" -ApiKey $apikey
