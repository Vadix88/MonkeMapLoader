# https://gist.github.com/Graicc/ad26b80ac1a733edc254053fdf012ffe
# Needs to be at least that version, or mmm can't read the archive
#Requires -Modules @{ ModuleName="Microsoft.PowerShell.Archive"; ModuleVersion="1.2.3" }
$MyInvocation.MyCommand.Path | Split-Path | Push-Location # Run from this script's directory
$Name = (ls *.csproj).BaseName
dotnet build -c Release
# mkdir BepInEx\plugins\$Name
curl -L https://github.com/Vadix88/MonkeMapLoader/releases/download/1.1.2/MonkeMapLoader-1.1.2.zip -o DL.zip
Expand-Archive DL.zip 
rm DL.zip
mv DL\BepInEx .
cp bin\Release\netstandard2.0\$Name.dll BepInEx\plugins\$Name\
Compress-Archive .\BepInEx\ $Name-v
rmdir .\BepInEx\ -Recurse
Pop-Location
