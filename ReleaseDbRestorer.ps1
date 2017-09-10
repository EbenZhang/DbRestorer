$ErrorActionPreference = 'Stop'
$projectPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("./DBRestorer/DBRestorer.csproj")
$csproj = [xml](Get-Content $projectPath)
$propertyGroup = $csproj.Project.PropertyGroup[0]

$thisRevision = [long]$propertyGroup.ApplicationRevision
$previousVersion = $propertyGroup.ApplicationVersion.Replace("%2a", $thisRevision - 1)
$folder = "DBRestorer_" + $previousVersion.Replace('.', '_')
$previousVersionFolder = join-path './dist/Application Files/' $folder

if(Test-Path $previousVersionFolder) {
    Remove-Item $previousVersionFolder -Force -Recurse
}
Remove-Item './dist/*.nupkg' -Force -Recurse

$propertyGroup.ApplicationRevision = ($thisRevision + 1).ToString()
write-host $propertyGroup.ApplicationRevision
$csproj.Save($projectPath)

git commit -am 'publish'
git push origin master
