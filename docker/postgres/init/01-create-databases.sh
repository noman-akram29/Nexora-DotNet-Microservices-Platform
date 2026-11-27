#!/usr/bin/env bash
set -Eeuo pipefail

: "${AUTH_DB_PASSWORD:?AUTH_DB_PASSWORD is required}"
: "${CUSTOMER_DB_PASSWORD:?CUSTOMER_DB_PASSWORD is required}"
: "${ORDER_DB_PASSWORD:?ORDER_DB_PASSWORD is required}"
: "${PAYMENT_DB_PASSWORD:?PAYMENT_DB_PASSWORD is required}"
: "${PRODUCT_DB_PASSWORD:?PRODUCT_DB_PASSWORD is required}"
: "${REPORT_DB_PASSWORD:?REPORT_DB_PASSWORD is required}"

psql \
  --set=ON_ERROR_STOP=1 \
  --username "$POSTGRES_USER" \
  --dbname "$POSTGRES_DB" \
  --set=auth_password="$AUTH_DB_PASSWORD" \
  --set=customer_password="$CUSTOMER_DB_PASSWORD" \
  --set=order_password="$ORDER_DB_PASSWORD" \
  --set=payment_password="$PAYMENT_DB_PASSWORD" \
  --set=product_password="$PRODUCT_DB_PASSWORD" \
  --set=report_password="$REPORT_DB_PASSWORD" <<'SQL'

CREATE ROLE auth_service LOGIN PASSWORD :'auth_password';
CREATE DATABASE auth_db OWNER auth_service;

CREATE ROLE customer_service LOGIN PASSWORD :'customer_password';
CREATE DATABASE customer_db OWNER customer_service;

CREATE ROLE order_service LOGIN PASSWORD :'order_password';
CREATE DATABASE order_db OWNER order_service;

CREATE ROLE payment_service LOGIN PASSWORD :'payment_password';
CREATE DATABASE payment_db OWNER payment_service;

CREATE ROLE product_service LOGIN PASSWORD :'product_password';
CREATE DATABASE product_db OWNER product_service;

CREATE ROLE report_service LOGIN PASSWORD :'report_password';
CREATE DATABASE report_db OWNER report_service;

SQL