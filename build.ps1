$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path $compiler)) { throw 'The built-in Windows C# compiler was not found.' }
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$output = Join-Path $root 'MotionCuesPrototype-v0.9-gui.exe'
$sources = Get-ChildItem (Join-Path $root 'src') -Filter '*.cs' | ForEach-Object { $_.FullName }
$windowsBase = Get-ChildItem (Join-Path $env:WINDIR 'Microsoft.NET\assembly') -Recurse -Filter 'WindowsBase.dll' | Select-Object -First 1 -ExpandProperty FullName
$presentationCore = Get-ChildItem (Join-Path $env:WINDIR 'Microsoft.NET\assembly') -Recurse -Filter 'PresentationCore.dll' | Select-Object -First 1 -ExpandProperty FullName
$systemXaml = Get-ChildItem (Join-Path $env:WINDIR 'Microsoft.NET\assembly') -Recurse -Filter 'System.Xaml.dll' | Select-Object -First 1 -ExpandProperty FullName
$presentationFramework = Get-ChildItem (Join-Path $env:WINDIR 'Microsoft.NET\assembly') -Recurse -Filter 'PresentationFramework.dll' | Select-Object -First 1 -ExpandProperty FullName
$arguments = @('/nologo', '/target:winexe', '/optimize+', '/platform:anycpu', ('/out:' + $output), '/reference:System.dll', '/reference:System.Core.dll', '/reference:System.Drawing.dll', '/reference:System.Windows.Forms.dll', ('/reference:' + $windowsBase), ('/reference:' + $presentationCore), ('/reference:' + $presentationFramework), ('/reference:' + $systemXaml)) + $sources
& $compiler $arguments
if ($LASTEXITCODE -ne 0) { throw "Compilation failed with exit code $LASTEXITCODE." }
Write-Host "Built MotionCuesPrototype-v0.9-gui.exe"





