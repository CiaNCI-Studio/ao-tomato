FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:7071
ENV DbSettings__ConnectionString=/data/aotomato.db

RUN mkdir /data && chmod 777 /data

EXPOSE 7071

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/ ./
RUN dotnet restore AoTomato.slnx
RUN dotnet build AoTomato.API/AoTomato.API.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish AoTomato.API/AoTomato.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN sed -i 's|_framework/blazor\.webassembly#\[\.{fingerprint}\]\.js|_framework/blazor.webassembly.js|g' /app/publish/wwwroot/index.html

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AoTomato.API.dll"]
