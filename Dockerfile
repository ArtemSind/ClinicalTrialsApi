FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClinicalTrialsApi.Api/ClinicalTrialsApi.Api.csproj", "ClinicalTrialsApi.Api/"]
COPY ["ClinicalTrialsApi.Core/ClinicalTrialsApi.Core.csproj", "ClinicalTrialsApi.Core/"]
COPY ["ClinicalTrialsApi.Infrastructure/ClinicalTrialsApi.Infrastructure.csproj", "ClinicalTrialsApi.Infrastructure/"]
RUN dotnet restore "ClinicalTrialsApi.Api/ClinicalTrialsApi.Api.csproj"
COPY . .
WORKDIR "/src/ClinicalTrialsApi.Api"
RUN dotnet build "ClinicalTrialsApi.Api.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "ClinicalTrialsApi.Api.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClinicalTrialsApi.Api.dll"] 