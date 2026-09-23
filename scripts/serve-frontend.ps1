param(
    [string]$Root = (Join-Path $PSScriptRoot '..\frontend'),
    [int]$Port = 5500
)

$ErrorActionPreference = 'Stop'
$Host.UI.RawUI.WindowTitle = 'ShopFlow Frontend'

$Root = (Resolve-Path $Root).Path.TrimEnd('\')
$rootPrefix = $Root + '\'

$mimeTypes = @{
    '.html'  = 'text/html; charset=utf-8'
    '.css'   = 'text/css; charset=utf-8'
    '.js'    = 'application/javascript; charset=utf-8'
    '.json'  = 'application/json; charset=utf-8'
    '.png'   = 'image/png'
    '.jpg'   = 'image/jpeg'
    '.jpeg'  = 'image/jpeg'
    '.gif'   = 'image/gif'
    '.svg'   = 'image/svg+xml'
    '.ico'   = 'image/x-icon'
    '.webp'  = 'image/webp'
    '.woff'  = 'font/woff'
    '.woff2' = 'font/woff2'
}

$listener = New-Object System.Net.HttpListener
$listener.Prefixes.Add("http://localhost:$Port/")

try {
    $listener.Start()
}
catch {
    Write-Host "Could not start the frontend server on port ${Port}:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Read-Host 'Press Enter to close'
    exit 1
}

Write-Host "ShopFlow frontend is served from $Root" -ForegroundColor Green
Write-Host "Open http://localhost:$Port/  -  close this window to stop." -ForegroundColor Green
Write-Host ''

try {
    while ($listener.IsListening) {
        $pending = $listener.BeginGetContext($null, $null)
        while (-not $pending.AsyncWaitHandle.WaitOne(500)) { }
        $context = $listener.EndGetContext($pending)

        $request = $context.Request
        $response = $context.Response

        try {
            $relative = [Uri]::UnescapeDataString($request.Url.AbsolutePath).TrimStart('/')
            if ([string]::IsNullOrEmpty($relative)) { $relative = 'index.html' }

            $path = [IO.Path]::GetFullPath((Join-Path $Root $relative))
            if (Test-Path -LiteralPath $path -PathType Container) {
                $path = Join-Path $path 'index.html'
            }

            if (-not $path.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                $response.StatusCode = 403
            }
            elseif (Test-Path -LiteralPath $path -PathType Leaf) {
                $extension = [IO.Path]::GetExtension($path).ToLowerInvariant()
                $contentType = $mimeTypes[$extension]
                if (-not $contentType) { $contentType = 'application/octet-stream' }

                $bytes = [IO.File]::ReadAllBytes($path)
                $response.StatusCode = 200
                $response.ContentType = $contentType
                $response.Headers['Cache-Control'] = 'no-cache'
                $response.ContentLength64 = $bytes.Length
                $response.OutputStream.Write($bytes, 0, $bytes.Length)
            }
            else {
                $response.StatusCode = 404
            }
        }
        catch {
            $response.StatusCode = 500
            Write-Host $_.Exception.Message -ForegroundColor Red
        }
        finally {
            Write-Host ("{0} {1} -> {2}" -f $request.HttpMethod, $request.Url.AbsolutePath, $response.StatusCode)
            $response.OutputStream.Close()
        }
    }
}
finally {
    $listener.Stop()
    $listener.Close()
}
