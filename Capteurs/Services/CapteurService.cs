using Capteurs.Data;
using Capteurs.Dtos;
using Capteurs.Entities;
using Capteurs.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Capteurs.Services
{
    /// <summary>
    /// Capteur Service
    /// </summary>
    public class CapteurService : ICapteurService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _memoryCache;

        private const string AllSensorsCacheKey = "__ALL_SENSORS__";
        private const string SingleSensorCacheKey = "__SENSOR_{0}__";


        public CapteurService(AppDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Récupérer tous les capteurs avec mise en cache
        /// </summary>
        public async Task<IEnumerable<CapteurDto>> GetAllCapteursAsync()
        {
            if (!_memoryCache.TryGetValue(AllSensorsCacheKey, out IEnumerable<CapteurDto> capteurDtos))
            {
                // Si la liste n'est pas en cache, on la charge depuis la base de données
                capteurDtos = await _context.Capteurs
                    .Where(c => c.IsActive)
                    .Select(s => new CapteurDto
                    {
                        Id = s.Id,
                        Nom = s.Nom,
                        Description = s.Description
                    })
                    .ToListAsync();

                // Mettre la liste dans le cache pour 10 minutes
                _memoryCache.Set(AllSensorsCacheKey, capteurDtos, TimeSpan.FromMinutes(10));
            }

            return capteurDtos;
        }

        /// <summary>
        /// Récupérer un capteur par ID avec mise en cache
        /// </summary>
        public async Task<CapteurDto?> GetCapteurByIdAsync(int id)
        {
            var cacheKey = string.Format(SingleSensorCacheKey, id);

            // Tente de récupérer le capteur depuis le cache
            if (!_memoryCache.TryGetValue(cacheKey, out CapteurDto? capteurDto))
            {
                // Si le capteur n'est pas dans le cache, on le cherche dans la base de données
                var capteur = await _context.Capteurs.FindAsync(id);
                if (capteur == null) return null;

                capteurDto = new CapteurDto
                {
                    Id = capteur.Id,
                    Nom = capteur.Nom,
                    Description = capteur.Description
                };

                // Mettre le capteur dans le cache pour 10 minutes
                _memoryCache.Set(cacheKey, capteurDto, TimeSpan.FromMinutes(10));
            }

            return capteurDto;
        }

        /// <summary>
        /// Créer un nouveau capteur et mettre à jour le cache
        /// </summary>
        public async Task<CapteurDto> AddCapteurAsync(CapteurDto capteurDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var capteur = new Capteur
                {
                    Nom = capteurDto.Nom,
                    Description = capteurDto.Description
                };

                _context.Capteurs.Add(capteur);
                await _context.SaveChangesAsync();

                // Invalider le cache des capteurs
                _memoryCache.Remove(AllSensorsCacheKey);

                // Créer un DTO pour retourner le capteur ajouté avec son ID
                var createdCapteurDto = new CapteurDto
                {
                    Id = capteur.Id,
                    Nom = capteur.Nom,
                    Description = capteur.Description
                };

                // Mettre le capteur dans le cache pour 10 minutes
                _memoryCache.Set(string.Format(SingleSensorCacheKey, capteur.Id), capteurDto, TimeSpan.FromMinutes(10));

                await transaction.CommitAsync();

                return createdCapteurDto;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Mettre à jour un capteur et mettre à jour le cache
        /// </summary>
        public async Task<bool> UpdateCapteurAsync(int id, CapteurDto capteurDto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var capteur = await _context.Capteurs.FindAsync(id);
                if (capteur == null) return false;

                capteur.Nom = capteurDto.Nom;
                capteur.Description = capteurDto.Description;

                await _context.SaveChangesAsync();

                // Invalider la liste cache des capteurs
                _memoryCache.Remove(AllSensorsCacheKey);
                // Invalider le cache du capteur spécifique
                _memoryCache.Remove(string.Format(SingleSensorCacheKey, id));
                // Mettre le capteur dans le cache pour 10 minutes
                _memoryCache.Set(string.Format(SingleSensorCacheKey, capteur.Id), capteurDto, TimeSpan.FromMinutes(10));

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Supprimer un capteur et mettre à jour le cache
        /// </summary>
        public async Task<bool> DeleteCapteurAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var capteur = await _context.Capteurs.FindAsync(id);
                if (capteur == null) return false;

                _context.Capteurs.Remove(capteur);
                await _context.SaveChangesAsync();

                // Invalider le cache des capteurs
                _memoryCache.Remove(AllSensorsCacheKey);
                // Invalider le cache du capteur spécifique
                _memoryCache.Remove(string.Format(SingleSensorCacheKey, id));

                // Confirmer la transaction
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                // Annuler la transaction en cas d'erreur
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Supprime un capteur de manière logique en le marquant comme inactif.
        /// </summary>
        /// <param name="id">Identifiant du capteur à archiver.</param>
        /// <returns>True si l'archivage a réussi, sinon False.</returns>
        public async Task<bool> ArchiveCapteurAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Rechercher le capteur par son identifiant
                var capteur = await _context.Capteurs.FindAsync(id);
                if (capteur == null) return false;

                // Marquer le capteur comme inactif
                capteur.IsActive = false;
                _context.Capteurs.Update(capteur);

                // Sauvegarder les modifications
                await _context.SaveChangesAsync();
                // Invalider le cache des capteurs
                _memoryCache.Remove(AllSensorsCacheKey);
                // Invalider le cache du capteur spécifique
                _memoryCache.Remove(string.Format(SingleSensorCacheKey, id));

                // Confirmer la transaction
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                // Annuler la transaction en cas d'erreur
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Restaure un capteur archivé en le marquant comme actif.
        /// </summary>
        /// <param name="id">Identifiant du capteur à restaurer.</param>
        /// <returns>True si la restauration a réussi, sinon False.</returns>
        public async Task<bool> RestoreCapteurAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Rechercher le capteur par son identifiant
                var capteur = await _context.Capteurs.FindAsync(id);
                if (capteur == null || capteur.IsActive) return false;

                // Marquer le capteur comme actif
                capteur.IsActive = true;
                _context.Capteurs.Update(capteur);

                // Sauvegarder les modifications
                await _context.SaveChangesAsync();
                // Invalider le cache des capteurs
                _memoryCache.Remove(AllSensorsCacheKey);
                // Invalider le cache du capteur spécifique
                _memoryCache.Remove(string.Format(SingleSensorCacheKey, id));
                // Confirmer la transaction
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                // Annuler la transaction en cas d'erreur
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
