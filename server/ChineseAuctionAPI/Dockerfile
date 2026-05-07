# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ChineseAuctionAPI/ChineseAuctionAPI.csproj ./ChineseAuctionAPI/
RUN dotnet restore ./ChineseAuctionAPI/ChineseAuctionAPI.csproj

# Copy the rest of the source code
COPY ChineseAuctionAPI/. ./ChineseAuctionAPI/

# Build the application
RUN dotnet publish ./ChineseAuctionAPI/ChineseAuctionAPI.csproj -c Release -o /app/publish

# Use the official ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set environment variables (optional)
# ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "ChineseAuctionAPI.dll"]
