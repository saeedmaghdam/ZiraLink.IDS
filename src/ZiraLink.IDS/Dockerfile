#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS restore
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["./src/ZiraLink.IDS/ZiraLink.IDS.csproj", "./src/ZiraLink.IDS/"]
RUN dotnet restore "./src/ZiraLink.IDS/ZiraLink.IDS.csproj"

FROM restore AS build
WORKDIR /src
COPY ./src/ ./src/
WORKDIR "./src/ZiraLink.IDS"
RUN dotnet build "ZiraLink.IDS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet clean
RUN dotnet publish "ZiraLink.IDS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y ca-certificates grep netcat-openbsd iputils-ping iproute2

COPY ./certs/*.pfx /app/certs/
COPY ./certs/*.crt /usr/local/share/ca-certificates/
COPY ./certs/*.key /usr/local/share/ca-certificates/
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "ZiraLink.IDS.dll"]
