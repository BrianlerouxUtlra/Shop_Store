# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Shop-store.csproj", "."]
RUN dotnet restore

# Copy the application code
COPY . .

# Exclude the test project from the build
RUN powershell -Command Remove-Item -Path .\Shop-store.Tests -Recurse -Force

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port the app runs on
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Start the application
ENTRYPOINT ["dotnet", "Shop-store.dll"]