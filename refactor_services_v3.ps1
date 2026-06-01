$serviceFiles = Get-ChildItem "ScadaServer.Application/Services/*.cs"

foreach ($file in $serviceFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # 1. Remove the infrastructure using
    $content = $content -replace 'using ScadaServer\.Infrastructure\.Repositories;\s*', ''
    
    # 2. Replace concrete repository with interface
    # This regex matches XRepository followed by whitespace and then a variable starting with _ or a letter
    $content = [regex]::Replace($content, '\b([A-Z][a-zA-Z]+)Repository\b', 'I$1Repository')

    # 3. Special Case: Some repos might have different names or manual overrides, but for this project they seem consistent.
    
    $content | Set-Content $file.FullName
}
