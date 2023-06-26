# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec {
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = 1)][scriptblock]$cmd,
        [Parameter(Position = 1, Mandatory = 0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if (Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

exec { & dotnet restore }

$suffix = $NULL
switch -Exact ($env:BUILD_TYPE)
{
    "PROD"
    {
        $suffix = "prod"; Break
    }
    "STAGE"
    {
        $suffix = "stage"; Break
    }
    Default {
        $suffix = "local"; Break
    }
}

$commitHash = $(git rev-parse --short HEAD)
$buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($commitHash)" }[$suffix -ne ""]

echo "build: Build version suffix is $buildSuffix"

exec { & dotnet build GroupDocs.Comparison.UI.sln -c Release --version-suffix=$buildSuffix -v q /nologo }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI\GroupDocs.Comparison.UI.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI.Api\GroupDocs.Comparison.UI.Api.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI.Core\GroupDocs.Comparison.UI.Core.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI.SelfHost.Api\GroupDocs.Comparison.UI.SelfHost.Api.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI.Api.Local.Cache\GroupDocs.Comparison.UI.Api.Local.Cache.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet pack .\src\GroupDocs.Comparison.UI.Api.Local.Storage\GroupDocs.Comparison.UI.Api.Local.Storage.csproj -c Release -o .\artifacts --include-symbols -p:SymbolPackageFormat=snupkg --no-build }
exec { & dotnet nuget sign .\artifacts\*.nupkg --certificate-path $env:PFX_PATH --certificate-password $env:PFX_PWD --timestamper $env:PFX_TMS }
exec { & dotnet nuget verify .\artifacts\*.nupkg --all }
