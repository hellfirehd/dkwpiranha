# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs npm \
    && rm -rf /var/lib/apt/lists/*

# Copy only project files first for better caching
COPY ["Directory.Build.props", "./"]
COPY ["dkw.io/dkw.io.csproj", "dkw.io/"]
COPY ["core/Piranha/Piranha.csproj", "core/Piranha/"]
COPY ["core/Piranha.AspNetCore/Piranha.AspNetCore.csproj", "core/Piranha.AspNetCore/"]
COPY ["core/Piranha.AttributeBuilder/Piranha.AttributeBuilder.csproj", "core/Piranha.AttributeBuilder/"]
COPY ["core/Piranha.ImageSharp/Piranha.ImageSharp.csproj", "core/Piranha.ImageSharp/"]
COPY ["core/Piranha.Local.FileStorage/Piranha.Local.FileStorage.csproj", "core/Piranha.Local.FileStorage/"]
COPY ["core/Piranha.Manager/Piranha.Manager.csproj", "core/Piranha.Manager/"]
COPY ["core/Piranha.Manager.LocalAuth/Piranha.Manager.LocalAuth.csproj", "core/Piranha.Manager.LocalAuth/"]
COPY ["core/Piranha.Manager.TinyMCE/Piranha.Manager.TinyMCE.csproj", "core/Piranha.Manager.TinyMCE/"]
COPY ["data/Piranha.Data.EF.SQLite/Piranha.Data.EF.SQLite.csproj", "data/Piranha.Data.EF.SQLite/"]
COPY ["identity/Piranha.AspNetCore.Identity.SQLite/Piranha.AspNetCore.Identity.SQLite.csproj", "identity/Piranha.AspNetCore.Identity.SQLite/"]
COPY ["dkw.io/package*.json", "dkw.io/"]

# Restore packages - this layer will be cached unless dependencies change
RUN dotnet restore "dkw.io/dkw.io.csproj" --disable-parallel
RUN cd dkw.io && if [ -f package-lock.json ]; then npm ci --ignore-scripts; else npm install --ignore-scripts; fi

# Copy source code
COPY . .

# Build and publish in one step
WORKDIR "/src/dkw.io"
RUN dotnet publish "dkw.io.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER root
RUN mkdir -p /app/wwwroot/uploads && chown -R $APP_UID:$APP_UID /app/wwwroot
USER $APP_UID
ENTRYPOINT ["dotnet", "dkw.io.dll"]
