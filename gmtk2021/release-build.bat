@echo off
if [%1]==[] goto usage

rmdir /Q /S .\bin\Release\netcoreapp3.1\win-x64\publish

dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained
del .\bin\Release\netcoreapp3.1\win-x64\publish\*.pdb

7z a -r .\%1.zip .\bin\Release\netcoreapp3.1\win-x64\publish\*
goto :eof


:usage
@echo Usage: %0 ^<NameOfBuild^>
pause
exit /B 1