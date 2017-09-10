$ErrorActionPreference = 'Stop'
$distFolder = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath('./dist/')
if(Test-Path $distFolder){
    Remove-Item $distFolder -Force -Recurse 
}
msbuild '/t:restore;build;publish' DBRestorer.sln /m /p:Configuration=Release
Copy-Item -Force -Recurse "./DBRestorer/bin/Release/app.publish" $distFolder
