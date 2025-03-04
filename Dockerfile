FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définition du répertoire de travail à l'intérieur du conteneur
WORKDIR /app

# Copier uniquement les fichiers du projet nécessaires pour restaurer les dépendances
COPY *.csproj ./
RUN dotnet restore

# Copier tous les fichiers et publier
COPY . .
RUN dotnet publish -c Release -o /app/out

# Image runtime à partir de l'aspnet de base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Exposer le port et démarrer l'appli
EXPOSE 80
ENTRYPOINT ["dotnet", "VoisinUp.dll"]