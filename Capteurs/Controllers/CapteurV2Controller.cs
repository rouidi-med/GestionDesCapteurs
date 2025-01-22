using Capteurs.Constants;
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
    [ApiExplorerSettings(GroupName = "v2")]
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
        /// <response code="200">Capteur archivé avec succès.</response>
        /// <response code="400">Données invalides fournies.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="404">Capteur non trouvé.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorId,
                    Message = "L'identifiant fourni est invalide."
                });
            }

            try
            {
                var deleted = await _sensorService.ArchiveCapteurAsync(id);
                if (!deleted)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = ErrorCode.SensorNotFound,
                        Message = $"Le capteur avec l'ID {id} est introuvable ou déjà archivé."
                    });
                }

                return Ok(new { message = $"Le capteur avec l'ID {id} a été archivé avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de l'archivage du capteur.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Restaure un capteur archivé.
        /// </summary>
        /// <param name="id">Identifiant du capteur.</param>
        /// <returns>Message sur le résultat de la restauration.</returns>
        /// <response code="200">Capteur restauré avec succès.</response>
        /// <response code="400">Données invalides fournies.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="404">Capteur non trouvé.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpPut("{id:int}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Restore(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorId,
                    Message = "L'identifiant fourni est invalide."
                });
            }

            try
            {
                var restored = await _sensorService.RestoreCapteurAsync(id);
                if (!restored)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = ErrorCode.SensorNotFound,
                        Message = $"Le capteur avec l'ID {id} est introuvable ou déjà actif."
                    });
                }

                return Ok(new { message = $"Le capteur avec l'ID {id} a été restauré avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorData,
                    Message = "Une erreur interne est survenue lors de la restauration du capteur.",
                    Details = ex.Message
                });
            }
        }
    }
}