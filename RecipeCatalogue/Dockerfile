FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /src

COPY RecipeCatalogue/*.csproj RecipeCatalogue/
COPY EventBusInterface/*.csproj EventBusInterface/
COPY EventBusImplementation/*.csproj EventBusImplementation/

RUN dotnet restore RecipeCatalogue/RecipeCatalogue.csproj
RUN dotnet restore EventBusInterface/EventBusInterface.csproj
RUN dotnet restore EventBusImplementation/EventBusImplementation.csproj

COPY . .

WORKDIR /src/RecipeCatalogue

RUN dotnet build "RecipeCatalogue.csproj" -o /app
RUN dotnet publish "RecipeCatalogue.csproj" -o /app

EXPOSE 80
WORKDIR /app
ENV DOTNET_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "RecipeCatalogue.dll"]