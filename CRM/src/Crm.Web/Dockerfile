# Multi-stage Dockerfile para Crm.Web (Blazor Server .NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Crm.slnx", "./"]
COPY ["src/Crm.Domain/Crm.Domain.csproj", "src/Crm.Domain/"]
COPY ["src/Crm.Application/Crm.Application.csproj", "src/Crm.Application/"]
COPY ["src/Crm.Infrastructure/Crm.Infrastructure.csproj", "src/Crm.Infrastructure/"]
COPY ["src/Crm.Web/Crm.Web.csproj", "src/Crm.Web/"]

RUN dotnet restore "src/Crm.Web/Crm.Web.csproj"

COPY . .
WORKDIR "/src/src/Crm.Web"
RUN dotnet publish "Crm.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Crm.Web.dll"]
