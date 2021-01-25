#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/DataMetrics.Api.Host/DataMetrics.Api.Host.csproj", "src/DataMetrics.Api.Host/"]
RUN dotnet restore "src/DataMetrics.Api.Host/DataMetrics.Api.Host.csproj"
COPY . .
WORKDIR "/src/src/DataMetrics.Api.Host"
RUN dotnet build "DataMetrics.Api.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataMetrics.Api.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataMetrics.Api.Host.dll"]
