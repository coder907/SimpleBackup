rmdir /s /q .\bin\Release\netcoreapp3.0\win-x64
dotnet clean -c Release
dotnet build -c Release
dotnet publish -c Release -r win-x64
del /s /q SimpleBackup.zip
rmdir /s /q SimpleBackup
md SimpleBackup
xcopy .\bin\Release\netcoreapp3.0\win-x64\publish .\SimpleBackup\
xcopy .\Config.json .\SimpleBackup\
"C:\Program Files\WinRAR\WinRAR.exe" a SimpleBackup.zip .\SimpleBackup
rmdir /s /q SimpleBackup
pause