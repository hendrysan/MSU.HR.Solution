#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MSU.HR.WebClient/MSU.HR.WebClient.csproj", "MSU.HR.WebClient/"]
COPY ["MSU.HR.Commons/MSU.HR.Commons.csproj", "MSU.HR.Commons/"]
COPY ["MSU.HR.Contexts/MSU.HR.Contexts.csproj", "MSU.HR.Contexts/"]
COPY ["MSU.HR.Models/MSU.HR.Models.csproj", "MSU.HR.Models/"]
COPY ["MSU.HR.Services/MSU.HR.Services.csproj", "MSU.HR.Services/"]
RUN dotnet restore "MSU.HR.WebClient/MSU.HR.WebClient.csproj"
COPY . .
WORKDIR "/src/MSU.HR.WebClient"
RUN dotnet build "MSU.HR.WebClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSU.HR.WebClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSU.HR.WebClient.dll"]