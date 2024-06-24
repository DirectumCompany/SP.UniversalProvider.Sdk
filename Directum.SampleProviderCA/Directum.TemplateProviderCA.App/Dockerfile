FROM registry.directum.ru/directum/rx-base:4.2.0-gost-crypto AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
ARG PAT=none
RUN dotnet nuget add source "https://irtfs.directum.ru/tfs/HRPro/HRPro/_packaging/CORE/nuget/v3/index.json" --name "CORE" --username "docker" --password "${PAT}" --store-password-in-clear-text --valid-authentication-types "basic"
COPY ["Directum.TemplateProviderCA.App/Directum.TemplateProviderCA.App.csproj", "Directum.TemplateProviderCA.App/"]
RUN dotnet restore "./Directum.TemplateProviderCA.App/Directum.TemplateProviderCA.App.csproj"
COPY ["Directum.TemplateProviderCA.App", "Directum.TemplateProviderCA.App"]
RUN dotnet build "Directum.TemplateProviderCA.App/Directum.TemplateProviderCA.App.csproj" -c Release --no-restore --no-self-contained

FROM build AS publish
RUN dotnet publish "Directum.TemplateProviderCA.App/Directum.TemplateProviderCA.App.csproj" -c Release -o /app/publish --no-restore --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Directum.TemplateProviderCA.App.dll"]
