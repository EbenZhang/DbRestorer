$ErrorActionPreference = 'Stop'
Remove-Item -Force ./dist/*nupkg -ErrorAction Ignore
nuget pack ./DBRestorer.Plugin.Interface/Nicologies.DBRestorer.Plugin.Interface.csproj -OutputDirectory ./dist -Prop Configuration=Release
