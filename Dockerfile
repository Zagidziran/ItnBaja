# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ITNBaja.sln ./

# Copy project files
COPY ITNBaja/ITNBaja/ITNBaja.csproj ITNBaja/ITNBaja/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR /src/ITNBaja/ITNBaja
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 9.0 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port 8080 (default for ASP.NET Core in containers)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "ITNBaja.dll"]