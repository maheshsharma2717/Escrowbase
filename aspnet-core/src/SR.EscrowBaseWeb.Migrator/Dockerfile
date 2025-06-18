#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SR.EscrowBaseWeb.Migrator/SR.EscrowBaseWeb.Migrator.csproj", "src/SR.EscrowBaseWeb.Migrator/"]
COPY ["src/SR.EscrowBaseWeb.EntityFrameworkCore/SR.EscrowBaseWeb.EntityFrameworkCore.csproj", "src/SR.EscrowBaseWeb.EntityFrameworkCore/"]
COPY ["src/SR.EscrowBaseWeb.Core/SR.EscrowBaseWeb.Core.csproj", "src/SR.EscrowBaseWeb.Core/"]
COPY ["src/SR.EscrowBaseWeb.Core.Shared/SR.EscrowBaseWeb.Core.Shared.csproj", "src/SR.EscrowBaseWeb.Core.Shared/"]
RUN dotnet restore "src/SR.EscrowBaseWeb.Migrator/SR.EscrowBaseWeb.Migrator.csproj"
COPY . .
WORKDIR "/src/src/SR.EscrowBaseWeb.Migrator"
RUN dotnet build "SR.EscrowBaseWeb.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SR.EscrowBaseWeb.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SR.EscrowBaseWeb.Migrator.dll"]
