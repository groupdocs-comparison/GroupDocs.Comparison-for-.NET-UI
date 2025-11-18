# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install libgdiplus
RUN apt update && apt install -y --no-install-recommends \ 
    libgdiplus \ 
    libc6-dev

# Install dependencies for Windows Fonts package
RUN apt update && apt install -y --no-install-recommends \
    fontconfig \
    cabextract \
    xfonts-utils \
    wget

# Download and install Windows Fonts package
RUN wget http://ftp.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.8.1_all.deb
RUN dpkg -i ttf-mscorefonts-installer_3.8.1_all.deb
RUN rm ttf-mscorefonts-installer_3.8.1_all.deb

# Update font cache
RUN fc-cache -f -v

# Cleanup temp files
RUN apt clean && rm -rf /var/lib/apt/lists/*

# Build 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["samples/GroupDocs.Comparison.UI.Sample/GroupDocs.Comparison.UI.Sample.csproj", "samples/GroupDocs.Comparison.UI.Sample/"]
COPY ["src/GroupDocs.Comparison.UI/GroupDocs.Comparison.UI.csproj", "src/GroupDocs.Comparison.UI/"]
COPY ["src/GroupDocs.Comparison.UI.Api/GroupDocs.Comparison.UI.Api.csproj", "src/GroupDocs.Comparison.UI.Api/"]
COPY ["src/GroupDocs.Comparison.UI.Core/GroupDocs.Comparison.UI.Core.csproj", "src/GroupDocs.Comparison.UI.Core/"]
COPY ["src/GroupDocs.Comparison.UI.Api.Local.Cache/GroupDocs.Comparison.UI.Api.Local.Cache.csproj", "src/GroupDocs.Comparison.UI.Api.Local.Cache/"]
COPY ["src/GroupDocs.Comparison.UI.Api.Local.Storage/GroupDocs.Comparison.UI.Api.Local.Storage.csproj", "src/GroupDocs.Comparison.UI.Api.Local.Storage/"]
COPY ["src/GroupDocs.Comparison.UI.SelfHost.Api/GroupDocs.Comparison.UI.SelfHost.Api.csproj", "src/GroupDocs.Comparison.UI.SelfHost.Api/"]

# Restore dependencies
RUN dotnet restore "./samples/GroupDocs.Comparison.UI.Sample/GroupDocs.Comparison.UI.Sample.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/samples/GroupDocs.Comparison.UI.Sample"

# Build the project
RUN dotnet build "./GroupDocs.Comparison.UI.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GroupDocs.Comparison.UI.Sample.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Entry point for the application
ENTRYPOINT ["dotnet", "GroupDocs.Comparison.UI.Sample.dll"]