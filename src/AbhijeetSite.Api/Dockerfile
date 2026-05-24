# Stage 1: Build the API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for reliable caching
COPY src/AbhijeetSite.ServiceDefaults/AbhijeetSite.ServiceDefaults.csproj src/AbhijeetSite.ServiceDefaults/
COPY src/AbhijeetSite.Api/AbhijeetSite.Api.csproj src/AbhijeetSite.Api/

# Restore dependencies
RUN dotnet restore src/AbhijeetSite.Api/AbhijeetSite.Api.csproj

# Copy the remaining source files
COPY src/AbhijeetSite.ServiceDefaults/ src/AbhijeetSite.ServiceDefaults/
COPY src/AbhijeetSite.Api/ src/AbhijeetSite.Api/

# Build and publish
WORKDIR /src/src/AbhijeetSite.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "AbhijeetSite.Api.dll"]
