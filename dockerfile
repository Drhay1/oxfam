# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy everything into the container
COPY . .

# Restore dependencies
RUN dotnet restore

# Publish the app (Release configuration)
RUN dotnet publish Oxfam.csproj -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/out ./

# Use the PORT environment variable Render sets
ENV ASPNETCORE_URLS=http://*:$PORT
EXPOSE $PORT

# Run the app
ENTRYPOINT ["dotnet", "Oxfam.dll"]