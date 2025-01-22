
# Solution de Gestion des Capteurs

## Table des Matières

1. [Description de l'API](#description-de-lapi)
2. [Sécurisation de l'API](#sécurisation-de-lapi)
3. [Documentation de l'API](#documentation-de-lapi)
4. [Tests](#tests)
5. [Installation et Démarrage](#installation-et-démarrage)
6. [Conclusion](#conclusion)

## Description de l'API

L'API permet de gérer des capteurs avec les opérations suivantes :

### **CapteurController** :
- **GET** `/api/capteur` : Récupère la liste de tous les capteurs.
- **GET** `/api/capteur/{id}` : Récupère un capteur spécifique par son identifiant.
- **POST** `/api/capteur` : Crée un nouveau capteur.
- **PUT** `/api/capteur/{id}` : Met à jour un capteur existant.
- **DELETE** `/api/capteur/{id}` : Supprime un capteur par son identifiant.

### **CapteurV2Controller** :
- **DELETE** `/api/v2/capteur/archive/{id}` : Supprime un capteur par son identifiant. Offre une suppression logique avec possibilité de restaurer. (amélioration de la suppression).
- **PUT** `/api/v2/capteur/restore/{id}` : Restaure un capteur archivé.

## Sécurisation de l'API

L'API est protégée par une clé API stockée dans le fichier `appsettings.json`. Chaque requête doit inclure cette clé pour être autorisée à accéder aux ressources.

### **Configuration de la clé API**

La clé API doit être ajoutée dans le fichier `appsettings.json` sous la section `ApiKey` :

```json
{
  "ApiKey": "votre_clé_api"
}
```

### **Forcer HTTPS**

Le site est configuré pour forcer l'utilisation du protocole HTTPS sur le port 5001. Cela garantit que toutes les connexions sont sécurisées.

### **Limitation du taux (Rate Limiting)**

L'API limite l'utilisation de ses endpoints à 10 requêtes par minute pour chaque utilisateur. Cela permet de prévenir les abus et de garantir une utilisation équitable des ressources.

## Documentation de l'API

L'API utilise **Swagger** pour documenter les points de terminaison et leurs réponses.

## Tests

Des tests unitaires et fonctionnels ont été ajoutés pour le point de terminaison **DELETE** afin de valider son bon fonctionnement. Ces tests couvrent les cas de suppression des capteurs et les erreurs potentielles comme les capteurs introuvables ou les erreurs internes.

## Installation et Démarrage

1. Clonez le dépôt de la solution.
2. Ouvrez la solution dans Visual Studio ou un éditeur compatible avec .NET 8.
3. Assurez-vous que les dépendances sont installées avec la commande suivante :

    ```bash
    dotnet restore
    ```

4. **Exécutez les migrations pour créer la base de données** avant de démarrer l'application :
   
    Exécutez la commande suivante pour appliquer les migrations :

    ```bash
    dotnet ef database update
    ```

    Cette commande appliquera toutes les migrations en attente et créera la base de données PostgreSQL.

5. Une fois les migrations exécutées, lancez l'application avec la commande suivante :

    ```bash
    dotnet run
    ```

   L'API sera accessible à l'adresse `https://localhost:5001/swagger/index.html`.

## Conclusion

Cette solution offre une gestion complète des capteurs avec une API sécurisée, documentée et performante. Les améliorations comme l'archivage des capteurs et la limitation du taux permettent de mieux contrôler l'accès et la manipulation des données.
