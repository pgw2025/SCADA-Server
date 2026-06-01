$files = Get-ChildItem -Path "ScadaServer.Application/Interfaces/*.cs", "ScadaServer.Application/Services/*.cs" -Include "*.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    $changed = $false
    
    # 1. Add using ScadaServer.Application.DTOs; if not present
    if ($content -notmatch "using ScadaServer.Application.DTOs;") {
        $content = "using ScadaServer.Application.DTOs;`r`n" + $content
        $changed = $true
    }

    # 2. Add using ScadaServer.Domain.Entities; if not present
    if ($content -notmatch "using ScadaServer.Domain.Entities;") {
        $content = "using ScadaServer.Domain.Entities;`r`n" + $content
        $changed = $true
    }
    
    if ($changed) {
        $content | Set-Content $file.FullName
    }
}
