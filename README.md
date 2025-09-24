# VoisinUp – Backend (Blazor Server + RepoDB + PostgreSQL)

Bienvenue dans le backend de VoisinUp ! 🎉  
Ce projet propulse l'API de l'application VoisinUp, une plateforme de gamification sociale entre voisins.

---

## 🚀 Lancer le projet

### ✅ Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgreSQL installé et fonctionnel
- La base de données configurée via `appsettings.json` ou une variable d’environnement
- (Facultatif) Visual Studio, Rider ou tout éditeur compatible .NET

### 🛠️ Étapes pour lancer le backend

```bash
cd VoisinUp
dotnet build
dotnet run
```

Par défaut, l'application s'exécute sur :

* http://localhost:80

### 🗄️ Base de données

Crée la base PostgreSQL avec le fichier DB_creation.sql fourni :

```bash
psql -U postgres -d voisinup -f DB_creation.sql
```

⚠️ Adapte `-U` et `-d` selon ton environnement.

### 🔐 Authentification

L’authentification utilise des tokens JWT.

Un token est généré à la connexion.

Il est requis dans l’en-tête `Authorization` des requêtes :

```makefile
Authorization: Bearer <token>
```

Tu peux modifier les paramètres JWT dans `Program.cs`.

### 🧪 Tester avec Swagger

Une fois le serveur lancé, accède à :

http://localhost:80/swagger

Pour explorer les routes de l’API de manière interactive.

### 📁 Structure

* `Controllers/` : routes HTTP
* `Services/` : logique métier
* `Repositories/` : accès à la base via RepoDB
* `Models/` : entités partagées