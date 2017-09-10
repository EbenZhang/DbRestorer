$ErrorActionPreference = 'Stop'
$distFolder = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath('./dist/')
if(Test-Path $distFolder){
    Remove-Item $distFolder -Force -Recurse 
}
msbuild '/t:restore;build;publish' DBRestorer.sln /p:Configuration=Release /p:PublishDir=$distFolder /m
