@echo off
cls

ECHO.
ECHO Building Primary Nuget Package BuildOnce
ECHO =======================================

nuget pack src\BuildOnce\BuildOnce.csproj -build -Prop Configuration=Release -IncludeReferencedProjects -OutputDirectory artifacts -Tool

ECHO.
ECHO All done
