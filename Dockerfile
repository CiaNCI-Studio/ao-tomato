FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ ./

RUN dotnet restore AoTomato.slnx
RUN dotnet publish AoTomato.Admin/AoTomato.Admin.csproj -c Release -o /admin-publish
RUN dotnet publish AoTomato.API/AoTomato.API.csproj -c Release -o /app
RUN mkdir -p /app/wwwroot && cp -r /admin-publish/wwwroot/* /app/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://+:7071
ENV DbSettings__ConnectionString=/data/aotomato.db

RUN mkdir /data && chmod 777 /data

EXPOSE 7071

ENTRYPOINT ["dotnet", "AoTomato.API.dll"]
