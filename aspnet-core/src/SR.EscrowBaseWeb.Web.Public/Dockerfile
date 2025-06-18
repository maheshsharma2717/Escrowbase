#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/SR.EscrowBaseWeb.Web.Public/SR.EscrowBaseWeb.Web.Public.csproj", "src/SR.EscrowBaseWeb.Web.Public/"]
COPY ["src/SR.EscrowBaseWeb.Web.Core/SR.EscrowBaseWeb.Web.Core.csproj", "src/SR.EscrowBaseWeb.Web.Core/"]
COPY ["src/SR.EscrowBaseWeb.Application/SR.EscrowBaseWeb.Application.csproj", "src/SR.EscrowBaseWeb.Application/"]
COPY ["src/SR.EscrowBaseWeb.Application.Shared/SR.EscrowBaseWeb.Application.Shared.csproj", "src/SR.EscrowBaseWeb.Application.Shared/"]
COPY ["src/SR.EscrowBaseWeb.Core.Shared/SR.EscrowBaseWeb.Core.Shared.csproj", "src/SR.EscrowBaseWeb.Core.Shared/"]
COPY ["src/SR.EscrowBaseWeb.Core/SR.EscrowBaseWeb.Core.csproj", "src/SR.EscrowBaseWeb.Core/"]
COPY ["src/SR.EscrowBaseWeb.EntityFrameworkCore/SR.EscrowBaseWeb.EntityFrameworkCore.csproj", "src/SR.EscrowBaseWeb.EntityFrameworkCore/"]
COPY ["src/SR.EscrowBaseWeb.GraphQL/SR.EscrowBaseWeb.GraphQL.csproj", "src/SR.EscrowBaseWeb.GraphQL/"]
RUN dotnet restore "src/SR.EscrowBaseWeb.Web.Public/SR.EscrowBaseWeb.Web.Public.csproj"
COPY . .
WORKDIR "/src/src/SR.EscrowBaseWeb.Web.Public"
RUN dotnet build "SR.EscrowBaseWeb.Web.Public.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SR.EscrowBaseWeb.Web.Public.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SR.EscrowBaseWeb.Web.Public.dll"]
