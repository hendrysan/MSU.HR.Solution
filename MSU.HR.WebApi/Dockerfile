#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MSU.HR.WebApi/MSU.HR.WebApi.csproj", "MSU.HR.WebApi/"]
COPY ["MSU.HR.Commons/MSU.HR.Commons.csproj", "MSU.HR.Commons/"]
COPY ["MSU.HR.Contexts/MSU.HR.Contexts.csproj", "MSU.HR.Contexts/"]
COPY ["MSU.HR.Models/MSU.HR.Models.csproj", "MSU.HR.Models/"]
COPY ["MSU.HR.Services/MSU.HR.Services.csproj", "MSU.HR.Services/"]
RUN dotnet restore "MSU.HR.WebApi/MSU.HR.WebApi.csproj"
COPY . .
WORKDIR "/src/MSU.HR.WebApi"
RUN dotnet build "MSU.HR.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSU.HR.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSU.HR.WebApi.dll"]