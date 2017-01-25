dotnet publish
cd .\bin\Debug\netstandard1.6\win10-x64\publish
Import-Module -Name C:\Users\Kent\Repository\Contented\Src\Contented.PowerShell\bin\Debug\netstandard1.6\win10-x64\publish\Contented.PowerShell.psd1
Write-Host "Good to go"

#Rename-Contented -Match "(?<name>.+)\.dll" -Replace "`${name}.exe"

cd \\Splinter\Videos\TV\Reality\Survivor\33\
Rename-Contented -Match ".*S33E(?<number>\d+).*\.mkv" -Replace "`${number}.mkv" -Recursive