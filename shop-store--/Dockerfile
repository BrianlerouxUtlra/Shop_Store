# Stage 1: Build the application on Windows
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2019 AS build-windows
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Shop-store.csproj", "."]
RUN dotnet restore

# Copy the application code
COPY . .

# Build the application
RUN dotnet build -c %BUILD_CONFIGURATION% -o /app/build

# Stage 2: Publish the application on Windows
FROM build-windows AS publish-windows
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# Stage 3: Build the application on Linux
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-linux
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Shop-store.csproj", "."]
RUN dotnet restore

# Copy the application code
COPY . .

# Build the application
RUN dotnet build -c %BUILD_CONFIGURATION% -o /app/build

# Stage 4: Publish the application on Linux
FROM build-linux AS publish-linux
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# Stage 5: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application from the appropriate platform
COPY --from=publish-windows /app/publish ./windows-publish/
COPY --from=publish-linux /app/publish ./linux-publish/

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Shop-store.dll"]