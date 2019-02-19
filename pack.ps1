## versioning
$major = $env:major;
$minor = $env:minor;
$patch = $env:patch;
$isVersioned = $env:isVersioned;
$isBetta = $env:isBetta;
$beta = "beta$env:BUILD_BUILDNUMBER";
$versionPartPrefix = '';
$versionPartSuffix = '';
## symbols
$includeSimbols = $env:includeSimbols;
$includeSimbolsStr = '';

## build config
$noBuild = $env:nobuildonpack;
$noBuildStr = '';

$buildConfig = $env:BUILDCONFIGURATION;

if ($noBuild)
{
    $noBuildStr = '--no-build';
}

if ($isVersioned)
{
    $versionPartPrefix = "/p:VersionPrefix=$major.$minor.$patch";
}

if($isBetta)
{
    $versionPartSuffix = "/p:VersionSuffix=$beta";
}

$test = Test-Path variable:global:$includeSimbols;

if ($includeSimbols)
{
    $includeSimbolsStr ='--include-symbols';
}


Write-Output "$major.$minor.$patch-$beta";

ForEach ($path in (Get-Childitem **/**.csproj -Recurse)) 
{
    #$path.FullName;
    Write-Output "dotnet pack $($path.FullName) -v n -c $buildConfig $versionPartPrefix $versionPartSuffix $includeSimbolsStr $noBuildStr";
    dotnet pack $path.FullName -v n -c $buildConfig $versionPartPrefix $versionPartSuffix $includeSimbolsStr $noBuildStr;
}
