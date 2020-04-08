FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app

COPY sources/*/*.csproj ./
RUN find . -name '*.csproj' -exec dotnet restore {} \;
RUN rm *.csproj
COPY ./ ./

FROM build-env as publish
RUN dotnet publish -c Release -o out sources/ToDo.Backend/ToDo.Backend.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=publish /app/out ./
ENTRYPOINT ["dotnet", "ToDo.Backend.dll"] 