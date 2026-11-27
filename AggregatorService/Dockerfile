FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["AggregatorService/AggregatorService.csproj", "AggregatorService/"]
COPY ["CommonService/CommonService.csproj", "CommonService/"]

RUN dotnet restore "AggregatorService/AggregatorService.csproj"

COPY . .
WORKDIR "/src/AggregatorService"
RUN dotnet build "AggregatorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AggregatorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER app
ENTRYPOINT ["dotnet", "AggregatorService.dll"]
