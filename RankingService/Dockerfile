FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src

COPY RankingService/*.csproj RankingService/
COPY EventBusInterface/*.csproj EventBusInterface/
COPY EventBusImplementation/*.csproj EventBusImplementation/
COPY SubsManagerInterface/*.csproj SubsManagerInterface/
COPY SubsManagerImplementation/*.csproj SubsManagerImplementation/ 

RUN dotnet restore RankingService/RankingService.csproj
RUN dotnet restore EventBusInterface/EventBusInterface.csproj
RUN dotnet restore EventBusImplementation/EventBusImplementation.csproj
RUN dotnet restore SubsManagerInterface/SubsManagerInterface.csproj
RUN dotnet restore SubsManagerImplementation/SubsManagerImplementation.csproj
COPY . .

WORKDIR /src/RankingService

RUN dotnet build "RankingService.csproj" -o /app
RUN dotnet publish "RankingService.csproj" -o /app

EXPOSE 80
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "RankingService.dll"]