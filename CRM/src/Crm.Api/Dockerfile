# Multi-stage Dockerfile para Crm.Api (.NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Crm.slnx", "./"]
COPY ["src/Crm.Domain/Crm.Domain.csproj", "src/Crm.Domain/"]
COPY ["src/Crm.Application/Crm.Application.csproj", "src/Crm.Application/"]
COPY ["src/Crm.Infrastructure/Crm.Infrastructure.csproj", "src/Crm.Infrastructure/"]
COPY ["src/Crm.Api/Crm.Api.csproj", "src/Crm.Api/"]

RUN dotnet restore "src/Crm.Api/Crm.Api.csproj"

COPY . .
WORKDIR "/src/src/Crm.Api"
RUN dotnet publish "Crm.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Crm.Api.dll"]
