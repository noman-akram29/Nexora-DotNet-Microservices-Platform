FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet tool install --global dotnet-ef --version 10.0.15

COPY . .

RUN dotnet restore ./MicroserviceShop.slnx

RUN mkdir -p /out && \
    dotnet ef migrations bundle --project AuthService/AuthService.csproj --startup-project AuthService/AuthService.csproj --context AuthDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-auth && \
    dotnet ef migrations bundle --project CustomerService/CustomerService.csproj --startup-project CustomerService/CustomerService.csproj --context CustomerDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-customer && \
    dotnet ef migrations bundle --project OrderService/OrderService.csproj --startup-project OrderService/OrderService.csproj --context OrderDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-order && \
    dotnet ef migrations bundle --project PaymentService/PaymentService.csproj --startup-project PaymentService/PaymentService.csproj --context PaymentDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-payment && \
    dotnet ef migrations bundle --project ProductService/ProductService.csproj --startup-project ProductService/ProductService.csproj --context ProductDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-product && \
    dotnet ef migrations bundle --project ReportService/ReportService.csproj --startup-project ReportService/ReportService.csproj --context ReportDbContext --configuration Release --target-runtime linux-x64 --output /out/nexora-migrate-report && \
    chmod 755 /out/nexora-migrate-*

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /out/ .

USER app
