FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PMS.Server/PMS.Server.csproj", "PMS.Server/"]
RUN dotnet restore "PMS.Server/PMS.Server.csproj"
COPY . .
WORKDIR "/src/PMS.Server"
RUN dotnet build "PMS.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PMS.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PMS.Server.dll"]