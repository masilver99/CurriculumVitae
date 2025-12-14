#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CV/CV.csproj", "CV/"]
RUN dotnet restore "CV/CV.csproj"
COPY . .
WORKDIR "/src/CV"
RUN dotnet build "CV.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CV.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "CV.dll"]