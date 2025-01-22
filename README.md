
# Solution de Gestion des Capteurs

## Table des Mati�res

1. [Description de l'API](#description-de-lapi)
2. [S�curisation de l'API](#s�curisation-de-lapi)
3. [Documentation de l'API](#documentation-de-lapi)
4. [Tests](#tests)
5. [Installation et D�marrage](#installation-et-d�marrage)
6. [Conclusion](#conclusion)

## Description de l'API

L'API permet de g�rer des capteurs avec les op�rations suivantes :

### **CapteurController** :
- **GET** `/api/capteur` : R�cup�re la liste de tous les capteurs.
- **GET** `/api/capteur/{id}` : R�cup�re un capteur sp�cifique par son identifiant.
- **POST** `/api/capteur` : Cr�e un nouveau capteur.
- **PUT** `/api/capteur/{id}` : Met � jour un capteur existant.
- **DELETE** `/api/capteur/{id}` : Supprime un capteur par son identifiant.

### **CapteurV2Controller** :
- **DELETE** `/api/v2/capteur/archive/{id}` : Supprime un capteur par son identifiant. Offre une suppression logique avec possibilit� de restaurer. (am�lioration de la suppression).
- **PUT** `/api/v2/capteur/restore/{id}` : Restaure un capteur archiv�.

## S�curisation de l'API

L'API est prot�g�e par une cl� API stock�e dans le fichier `appsettings.json`. Chaque requ�te doit inclure cette cl� pour �tre autoris�e � acc�der aux ressources.

### **Configuration de la cl� API**

La cl� API doit �tre ajout�e dans le fichier `appsettings.json` sous la section `ApiKey` :

```json
{
  "ApiKey": "votre_cl�_api"
}
```

### **Forcer HTTPS**

Le site est configur� pour forcer l'utilisation du protocole HTTPS sur le port 5001. Cela garantit que toutes les connexions sont s�curis�es.

### **Limitation du taux (Rate Limiting)**

L'API limite l'utilisation de ses endpoints � 10 requ�tes par minute pour chaque utilisateur. Cela permet de pr�venir les abus et de garantir une utilisation �quitable des ressources.

## Documentation de l'API

L'API utilise **Swagger** pour documenter les points de terminaison et leurs r�ponses.

## Tests

Des tests unitaires et fonctionnels ont �t� ajout�s pour le point de terminaison **DELETE** afin de valider son bon fonctionnement. Ces tests couvrent les cas de suppression des capteurs et les erreurs potentielles comme les capteurs introuvables ou les erreurs internes.

## Installation et D�marrage

1. Clonez le d�p�t de la solution.
2. Ouvrez la solution dans Visual Studio ou un �diteur compatible avec .NET 8.
3. Assurez-vous que les d�pendances sont install�es avec la commande suivante :

    ```bash
    dotnet restore
    ```

4. **Ex�cutez les migrations pour cr�er la base de donn�es** avant de d�marrer l'application :
   
    Ex�cutez la commande suivante pour appliquer les migrations :

    ```bash
    dotnet ef database update
    ```

    Cette commande appliquera toutes les migrations en attente et cr�era la base de donn�es PostgreSQL.

5. Une fois les migrations ex�cut�es, lancez l'application avec la commande suivante :

    ```bash
    dotnet run
    ```

   L'API sera accessible � l'adresse `https://localhost:5001/swagger/index.html`.

## Conclusion

Cette solution offre une gestion compl�te des capteurs avec une API s�curis�e, document�e et performante. Les am�liorations comme l'archivage des capteurs et la limitation du taux permettent de mieux contr�ler l'acc�s et la manipulation des donn�es.
