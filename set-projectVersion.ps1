$versionPattern = '^\[assembly: AssemblyVersion\("(.*)"\)\]'
$path = '.\src\SharedAssemblyInfo.cs'

(Get-Content $path) | ForEach-Object{
    if($_ -match $versionPattern){
        $projectVersion = [version]$matches[1]
        $versionString = "{0}.{1}.{2}" -f $projectVersion.Major, $projectVersion.Minor, $projectVersion.Build
        Write-Host "##vso[task.setvariable variable=projectVersion]$versionString"
    }
}
