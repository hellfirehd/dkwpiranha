# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dkw.io/dkw.io.csproj", "dkw.io/"]
COPY ["core/Piranha.AspNetCore/Piranha.AspNetCore.csproj", "core/Piranha.AspNetCore/"]
COPY ["core/Piranha.AttributeBuilder/Piranha.AttributeBuilder.csproj", "core/Piranha.AttributeBuilder/"]
COPY ["core/Piranha.ImageSharp/Piranha.ImageSharp.csproj", "core/Piranha.ImageSharp/"]
COPY ["core/Piranha.Local.FileStorage/Piranha.Local.FileStorage.csproj", "core/Piranha.Local.FileStorage/"]
COPY ["core/Piranha.Manager.LocalAuth/Piranha.Manager.LocalAuth.csproj", "core/Piranha.Manager.LocalAuth/"]
COPY ["core/Piranha.Manager.TinyMCE/Piranha.Manager.TinyMCE.csproj", "core/Piranha.Manager.TinyMCE/"]
COPY ["core/Piranha.Manager/Piranha.Manager.csproj", "core/Piranha.Manager/"]
COPY ["core/Piranha/Piranha.csproj", "core/Piranha/"]
COPY ["data/Piranha.Data.EF.SQLite/Piranha.Data.EF.SQLite.csproj", "data/Piranha.Data.EF.SQLite/"]
COPY ["identity/Piranha.AspNetCore.Identity.SQLite/Piranha.AspNetCore.Identity.SQLite.csproj", "identity/Piranha.AspNetCore.Identity.SQLite/"]
COPY ["Directory.Build.props", ""]
RUN dotnet restore "./dkw.io/dkw.io.csproj"
COPY . .
WORKDIR "/src/dkw.io"
RUN dotnet build "./dkw.io.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dkw.io.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN mkdir -p /app/wwwroot/uploads && chown -R $APP_UID:$APP_UID /app/wwwroot
USER $APP_UID
ENTRYPOINT ["dotnet", "dkw.io.dll"]
