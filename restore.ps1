$major = $env:major;
$minor = $env:minor;
$patch = $env:patch;
$isVersioned = $env:isVersioned;
$isBetta = $env:isBetta;
$beta = "beta$env:BUILD_BUILDNUMBER";
$versionPartPrefix = ''
$versionPartSuffix = ''

if ($isVersioned)
{
    $versionPartPrefix = "/p:VersionPrefix=$major.$minor.$patch";
}

if($isBetta)
{
    $versionPartSuffix = "/p:VersionSuffix=$beta";
}


Write-Output "$major.$minor.$patch-$beta"


ForEach ($path in (Get-Childitem **/**.csproj -Recurse)) 
{
    Write-Output "dotnet restore $($path.FullName) $versionPartPrefix $versionPartSuffix"
    dotnet restore $path.FullName $versionPartPrefix $versionPartSuffix;
}
