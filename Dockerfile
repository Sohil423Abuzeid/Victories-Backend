# official .NET runtime as the base image (for running the app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:5000
WORKDIR /app
EXPOSE 80


# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InstaHub.csproj", "./"]
RUN dotnet restore "./InstaHub.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "InstaHub.csproj" -c Release -o /app/build

# Publish the app to be run in the final image
FROM build AS publish
RUN dotnet publish "InstaHub.csproj" -c Release -o /app/publish

# Use the runtime image again for the final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InstaHub.dll"]

