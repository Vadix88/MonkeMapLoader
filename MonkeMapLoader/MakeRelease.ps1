$Name = "MonkeMapLoader" # Replace with your mods name
curl -L https://github.com/Vadix88/MonkeMapLoader/releases/download/1.1.2/MonkeMapLoader-1.1.2.zip -o DL.zip
Expand-Archive DL.zip 
rm DL.zip
dotnet build -c Release -o Temp
cp Temp\$Name.dll DL\BepInEx\plugins\$Name\$Name.dll
rmdir Temp -Recurse
Compress-Archive DL\BepInEx\ $Name-v.zip 
rmdir DL -Recurse
