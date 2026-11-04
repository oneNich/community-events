FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CommunityEvents.csproj", "."]
RUN dotnet restore "CommunityEvents.csproj"
COPY . .
RUN dotnet build "CommunityEvents.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommunityEvents.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommunityEvents.dll"]
