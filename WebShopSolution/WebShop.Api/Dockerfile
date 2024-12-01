FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App/out .

# Expose port and define entry point
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "WebShop.Api.dll"]


# Stage 1: Build all dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["WebShop.Api/WebShop.Api.csproj", "./WebShop.Api/"]
COPY ["WebShop.Application/WebShop.Application.csproj", "./WebShop.Application/"]
COPY ["WebShop.Domain/WebShop.Domain.csproj", "./WebShop.Domain/"]
COPY ["WebShop.Infrastructure/WebShop.Infrastructure.csproj", "./WebShop.Infrastructure/"]
COPY ["WebShopTests/WebShopTests.csproj", "./WebShopTests/"]


# Restore dependencies
RUN dotnet restore "WebShop.Api/WebShop.Api.csproj"

# Copy the full solution
COPY . .

# Build all projects
RUN dotnet build "WebShop.Api/WebShop.Api.csproj" -c Release

# Stage 2: Run tests
FROM build AS test
WORKDIR /src/WebShopTests

# Run tests and ensure they pass
RUN dotnet test "WebShopTests.csproj" --no-build --logger:trx --results-directory /src/TestResults

# Stage 3: Publish WebShop (only proceeds if tests pass)
FROM build AS publish
WORKDIR /src/WebShop.Api
RUN dotnet publish "WebShop.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image for WebShop
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShop.Api.dll"]