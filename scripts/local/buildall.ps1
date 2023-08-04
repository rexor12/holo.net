$sourceDirectory = [System.IO.Path]::Combine($PWD, "src")
Write-Host "Source directory: $sourceDirectory"

Write-Host "Building Holo.sln..."
& dotnet.exe build Holo.sln
Write-Host "Successfully built Holo.sln"

$projectDirectories = Get-ChildItem -Directory -Path $sourceDirectory | Select-Object -ExpandProperty FullName

$moduleNamePattern = "Holo\.Module\.(?<ModuleName>\w+)$"
foreach ($projectDirectory in $projectDirectories) {
    $match = [System.Text.RegularExpressions.Regex]::Match($projectDirectory, $moduleNamePattern)
    if (!$match.Success) {
        # Write-Host "Ignoring non-module folder '$projectDirectory'"
        continue
    }

    $moduleName = $match.Groups["ModuleName"]
    Write-Host "Copying module files of '$moduleName'..."
    $binPath = [System.IO.Path]::Combine($projectDirectory, "bin\Debug\net7.0")
    $targetBinPath = [System.IO.Path]::Combine($sourceDirectory, "Holo.ServiceHost\bin\Debug\net7.0\Modules\$moduleName")
    if (![System.IO.Directory]::Exists($targetBinPath)) {
        [System.IO.Directory]::CreateDirectory($targetBinPath)
    }

    Copy-Item -Path "$binPath\*" -Destination $targetBinPath -Recurse -Force
    Write-Host "Successfully copied module files of '$moduleName'"
}

# Copy-Item -Path $sourceFolder\* -Destination $destinationFolder -Recurse -Force