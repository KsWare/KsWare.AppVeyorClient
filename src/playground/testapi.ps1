$token = 'your token'
$headers = @{}
$headers['Authorization'] = "Bearer $token"
$headers["Content-type"] = "application/json"
Invoke-RestMethod -Uri 'https://ci.appveyor.com/api/users' -Headers $headers -Method Get