using Capteurs.Dtos;
using Capteurs.Filters;
using Capteurs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Capteurs.Controllers
{
    /// <summary>
    /// Contrôleur pour gérer les capteurs.
    /// </summary>
    [ApiController]
    [Route("api/v2/capteur")]
    [ServiceFilter(typeof(ApiKeyAuthAttribute))]
    public class CapteurV2Controller(ICapteurService sensorService) : ControllerBase
    {
        private readonly ICapteurService _sensorService = sensorService;

        /// <summary>
        /// Supprime un capteur par son identifiant.
        /// Offre une suppression logique avec possibilité de restaurer.
        /// </summary>
        /// <param name="id">Identifiant du capteur.</param>
        /// <returns>Message sur le résultat de la suppression.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "L'identifiant fourni est invalide." });
            }

            try
            {

                var deleted = await _sensorService.ArchiveCapteurAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Le capteur avec l'ID {id} est introuvable ou déjà archivé." });
                }

                return Ok(new { message = $"Le capteur avec l'ID {id} a été archivé avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", erreur = ex.Message });
            }
        }

        /// <summary>
        /// Restaure un capteur archivé.
        /// </summary>
        /// <param name="id">Identifiant du capteur.</param>
        /// <returns>Message sur le résultat de la restauration.</returns>
        [HttpPut("{id:int}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "L'identifiant fourni est invalide." });
            }

            try
            {
                var restored = await _sensorService.RestoreCapteurAsync(id);
                if (!restored)
                {
                    return NotFound(new { message = $"Le capteur avec l'ID {id} est introuvable ou déjà actif." });
                }

                return Ok(new { message = $"Le capteur avec l'ID {id} a été restauré avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", erreur = ex.Message });
            }
        }
    }
}