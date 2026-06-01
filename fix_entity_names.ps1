$files = Get-ChildItem -Path . -Include *.cs -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match "DeviceEntity") {
        $content = $content -replace "DeviceEntity", "Device"
        $content | Set-Content $file.FullName
    }
}
