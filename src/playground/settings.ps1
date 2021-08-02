$token = 'xqssufyvei7xfsp1b5v5'
$headers = @{}
$headers['Authorization'] = "Bearer $token"
$headers["Content-type"] = "application/json"
Invoke-RestMethod -Uri 'https://ci.appveyor.com/api/projects/KsWare/ksware-presentation/settings' -Headers $headers -Method Get
Write-Host "Press any key to continue..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")