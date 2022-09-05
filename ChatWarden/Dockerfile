FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ChatWarden/ChatWarden.csproj", "ChatWarden/"]
RUN dotnet restore "ChatWarden/ChatWarden.csproj"
COPY . .
WORKDIR "/src/ChatWarden"
RUN dotnet build "ChatWarden.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatWarden.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatWarden.dll"]