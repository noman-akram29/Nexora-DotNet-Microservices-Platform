#!/usr/bin/env bash
# Sets local dev secrets with `dotnet user-secrets` (stored outside the repo).
# Usage: POSTGRES_PASSWORD=password ./scripts/set-dev-secrets.sh
set -euo pipefail
cd "$(dirname "$0")/.."
PG="${POSTGRES_PASSWORD:-password}"
MINIO_USER="${MINIO_ROOT_USER:-minioadmin}"; MINIO_PASS="${MINIO_ROOT_PASSWORD:-minioadmin}"
JWT="$(openssl rand -base64 64 | tr -d '\n')"          # shared: Auth, Gateway, Order, Aggregator
JWT_REFRESH="$(openssl rand -base64 64 | tr -d '\n')"  # Auth only
conn() { echo "Host=localhost;Port=5555;Database=$1;Username=postgres;Password=$PG"; }
set_s() { dotnet user-secrets set "$2" "$3" --project "$1" >/dev/null; echo "  $1: $2"; }
init() { dotnet user-secrets init --project "$1" >/dev/null; }
for p in AuthService ApiGateway ProductService OrderService PaymentService CustomerService ReportService NotificationService AggregatorService; do init $p; done
set_s AuthService ConnectionStrings:AuthDb "$(conn AuthDb)"
set_s AuthService Jwt:SecretKey "$JWT"
set_s AuthService Jwt:RefreshSecretKey "$JWT_REFRESH"
set_s ApiGateway Jwt:SecretKey "$JWT"
set_s OrderService Jwt:SecretKey "$JWT"
set_s AggregatorService JwtSetting:securityKey "$JWT"
set_s ProductService ConnectionStrings:Default "$(conn ProductDb)"
set_s OrderService ConnectionStrings:Default "$(conn OrderDb)"
set_s PaymentService ConnectionStrings:Default "$(conn PaymentDb)"
set_s CustomerService ConnectionStrings:Default "$(conn CustomerDb)"
set_s ReportService ConnectionStrings:Default "$(conn ReportDb)"
for s in ProductService CustomerService; do set_s $s MinioSettings:AccessKey "$MINIO_USER"; set_s $s MinioSettings:SecretKey "$MINIO_PASS"; done
echo "NotificationService SMTP is NOT set. Run:"
echo "  dotnet user-secrets set Smtp:Username <you@gmail.com> --project NotificationService"
echo "  dotnet user-secrets set Smtp:Password <NEW app password> --project NotificationService"
echo "  dotnet user-secrets set Smtp:FromEmail <you@gmail.com> --project NotificationService"
