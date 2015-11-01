param(
    [string]$Configuration = "Release"
)

$msbuild = "C:\Program Files (x86)\MSBuild\14.0\bin\msbuild.exe"
& $msbuild $PSScriptRoot\AerialForWindows.sln /p:Configuration=$Configuration /m

$AerialForWindowsScr = "$PSScriptRoot\AerialForWindows\bin\$Configuration\AerialForWindows.scr"
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($AerialForWindowsScr).ProductVersion

$packages = [xml] (Get-Content "$PSScriptRoot\AerialForWindows\packages.config")
$wixPackage = $packages.packages.package | ? { $_.id -eq 'WiX' }
$wixPath = [IO.Path]::Combine($PSScriptRoot, 'packages', "WiX.$($wixPackage.version)", 'tools')

$wixPath

& "$wixPath\candle.exe" "Setup\AerialForWindows.wxs" -out "Setup\obj\$Configuration\AerialForWindows.wixobj" -dConfiguration="$Configuration"
& "$wixPath\light.exe" -ext WixNetFxExtension -ext WixUIExtension -ext WixUtilExtension -out "setup\bin\$Configuration\AerialForWindows-$Version.msi" "Setup\obj\$Configuration\AerialForWindows.wixobj"
