# Windows version. Usage: .\scripts\set-dev-secrets.ps1 [-PostgresPassword password]
param([string]$PostgresPassword="password",[string]$MinioUser="minioadmin",[string]$MinioPass="minioadmin")
$ErrorActionPreference="Stop"; Set-Location (Join-Path $PSScriptRoot "..")
function NewKey { $b=New-Object byte[] 64; $r=[Security.Cryptography.RandomNumberGenerator]::Create(); $r.GetBytes($b); $r.Dispose(); [Convert]::ToBase64String($b) }
$jwt=NewKey; $refresh=NewKey
function Conn($db){"Host=localhost;Port=5555;Database=$db;Username=postgres;Password=$PostgresPassword"}
function S($p,$k,$v){ dotnet user-secrets set $k $v --project $p | Out-Null; Write-Host "  ${p}: $k" }
foreach($p in "AuthService","ApiGateway","ProductService","OrderService","PaymentService","CustomerService","ReportService","NotificationService","AggregatorService"){ dotnet user-secrets init --project $p | Out-Null }
S AuthService ConnectionStrings:AuthDb (Conn AuthDb); S AuthService Jwt:SecretKey $jwt; S AuthService Jwt:RefreshSecretKey $refresh
S ApiGateway Jwt:SecretKey $jwt; S OrderService Jwt:SecretKey $jwt; S AggregatorService JwtSetting:securityKey $jwt
S ProductService ConnectionStrings:Default (Conn ProductDb); S OrderService ConnectionStrings:Default (Conn OrderDb)
S PaymentService ConnectionStrings:Default (Conn PaymentDb); S CustomerService ConnectionStrings:Default (Conn CustomerDb); S ReportService ConnectionStrings:Default (Conn ReportDb)
foreach($s in "ProductService","CustomerService"){ S $s MinioSettings:AccessKey $MinioUser; S $s MinioSettings:SecretKey $MinioPass }
Write-Host "Set SMTP yourself: dotnet user-secrets set Smtp:Password <NEW app password> --project NotificationService (also Smtp:Username, Smtp:FromEmail)"
