$ErrorActionPreference = 'Stop'
$distFolder = resolve-path 'dist\'
Remove-Item $distFolder -Force -Recurse
msbuild '/t:restore;build;publish' DBRestorer.sln /p:Configuration=Release /p:PublishDir=$distFolder /m
