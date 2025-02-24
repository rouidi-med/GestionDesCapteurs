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
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiKeyAuthAttribute))]
    public class CapteurController : ControllerBase
    {
        private readonly ICapteurService _sensorService;

        public CapteurController(ICapteurService sensorService)
        {
            _sensorService = sensorService;
        }

        /// <summary>
        /// Récupère tous les capteurs.
        /// </summary>
        /// <returns>La liste des capteurs.</returns>
        /// <response code="200">Liste des capteurs récupérée avec succès.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CapteurDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var sensors = await _sensorService.GetAllCapteursAsync();
                return Ok(sensors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de la récupération des capteurs.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Récupère un capteur spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur.</param>
        /// <returns>Le capteur demandé.</returns>
        /// <response code="200">Capteur récupéré avec succès.</response>
        /// <response code="400">Données invalides.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="404">Capteur non trouvé.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CapteurDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorId,
                    Message = "L'ID du capteur doit être supérieur à zéro."
                });
            }

            try
            {
                var sensor = await _sensorService.GetCapteurByIdAsync(id);
                if (sensor == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = ErrorCode.SensorNotFound,
                        Message = $"Le capteur avec l'ID {id} est introuvable."
                    });
                }

                return Ok(sensor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de la récupération du capteur.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Ajoute un nouveau capteur.
        /// </summary>
        /// <param name="sensorDto">Les détails du capteur à ajouter.</param>
        /// <returns>Le capteur ajouté.</returns>
        /// <response code="201">Capteur créé avec succès.</response>
        /// <response code="400">Données invalides fournies.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CapteurDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> Create([FromBody] CapteurDto sensorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorData,
                    Message = "Les données du capteur fournies sont invalides.",
                    Details = ModelState.ToString()
                });
            }

            try
            {
                var createdSensor = await _sensorService.AddCapteurAsync(sensorDto);
                return CreatedAtAction(nameof(GetById), new { id = createdSensor.Id }, createdSensor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de la création du capteur.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Met à jour un capteur existant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur à mettre à jour.</param>
        /// <param name="sensorDto">Les nouveaux détails du capteur.</param>
        /// <returns>Aucune réponse si la mise à jour est réussie.</returns>
        /// <response code="204">Mise à jour réussie.</response>
        /// <response code="400">Données invalides.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="404">Capteur non trouvé.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> Update(int id, [FromBody] CapteurDto sensorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorData,
                    Message = "Les données du capteur fournies sont invalides.",
                    Details = ModelState.ToString()
                });
            }

            try
            {
                var updated = await _sensorService.UpdateCapteurAsync(id, sensorDto);
                if (!updated)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = ErrorCode.SensorNotFound,
                        Message = $"Le capteur avec l'ID {id} est introuvable."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de la mise à jour du capteur.",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Supprime un capteur par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur à supprimer.</param>
        /// <returns>Aucune réponse si la suppression est réussie.</returns>
        /// <response code="204">Suppression réussie.</response>
        /// <response code="400">Données invalides.</response>
        /// <response code="401">Informations d'authentification manquantes ou invalides.</response>
        /// <response code="404">Capteur non trouvé.</response>
        /// <response code="429">Erreur liée à la limite de quota</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesErrorResponseType(typeof(ErrorResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = ErrorCode.InvalidSensorId,
                    Message = "L'ID du capteur doit être supérieur à zéro."
                });
            }

            try
            {
                var deleted = await _sensorService.DeleteCapteurAsync(id);
                if (!deleted)
                {
                    return NotFound(new ErrorResponse
                    {
                        Code = ErrorCode.SensorNotFound,
                        Message = $"Le capteur avec l'ID {id} est introuvable."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Code = ErrorCode.InternalServerError,
                    Message = "Une erreur interne est survenue lors de la suppression du capteur.",
                    Details = ex.Message
                });
            }
        }
    }
}
