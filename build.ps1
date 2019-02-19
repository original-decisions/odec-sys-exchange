##versioning via build 
$major = $env:major;
$minor = $env:minor;
$patch = $env:patch;
$isVersioned = $env:isVersioned;
$isBetta = $env:isBetta;
$beta = "beta$env:BUILD_BUILDNUMBER";
$versionPartPrefix = '';
$versionPartSuffix = '';

## build config
$buildConfig = $env:BUILDCONFIGURATION;

$buildNoRestore= $env:norestoreonbuild;
$buildNoRestoreStr = '';



if ($buildNoRestore)
{
    $buildNoRestoreStr = '--no-restore';
}


if ($isVersioned)
{
    $versionPartPrefix = "/p:VersionPrefix=$major.$minor.$patch";
}

if($isBetta)
{
    $versionPartSuffix = "/p:VersionSuffix=$beta";
}


Write-Output "$major.$minor.$patch-$beta";

ForEach ($path in (Get-Childitem **/**.csproj -Recurse)) 
{
    #$path.FullName;
    Write-Output "dotnet build $($path.FullName) -v n -c $buildConfig $versionPartPrefix $versionPartSuffix $buildNoRestoreStr";
    dotnet build $path.FullName -v n -c $buildConfig $versionPartPrefix $versionPartSuffix $buildNoRestoreStr;
}
