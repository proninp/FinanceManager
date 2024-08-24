FROM git.ivanin-kv.ru/skunkworks/dotnet/sdk:8.0-alpine AS build

COPY . /source

WORKDIR /source/FinanceManager/FinanceManager.API


RUN dotnet publish -c Release -o /app

FROM git.ivanin-kv.ru/skunkworks/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app .
USER $APP_UID

ENTRYPOINT ["dotnet", "FinanceManager.API.dll"]
