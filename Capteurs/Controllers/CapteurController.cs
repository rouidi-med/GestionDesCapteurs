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
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiKeyAuthAttribute))]
    public class CapteurController(ICapteurService sensorService) : ControllerBase
    {
        private readonly ICapteurService _sensorService = sensorService;

        /// <summary>
        /// Récupère tous les capteurs.
        /// </summary>
        /// <returns>La liste des capteurs.</returns>
        /// <response code="200">Liste des capteurs récupérée avec succès.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CapteurDto>))]
        public async Task<IActionResult> GetAll()
        {
            var sensors = await _sensorService.GetAllCapteursAsync();
            return Ok(sensors);
        }

        /// <summary>
        /// Récupère un capteur spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur.</param>
        /// <returns>Le capteur demandé.</returns>
        /// <response code="200">Capteur récupéré avec succès.</response>
        /// <response code="404">Capteur non trouvé.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CapteurDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var sensor = await _sensorService.GetCapteurByIdAsync(id);
            if (sensor == null)
                return NotFound();

            return Ok(sensor);
        }

        /// <summary>
        /// Ajoute un nouveau capteur.
        /// </summary>
        /// <param name="sensorDto">Les détails du capteur à ajouter.</param>
        /// <returns>Le capteur ajouté.</returns>
        /// <response code="201">Capteur créé avec succès.</response>
        /// <response code="400">Données invalides fournies.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CapteurDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CapteurDto sensorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSensor = await _sensorService.AddCapteurAsync(sensorDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSensor.Id }, createdSensor);
        }

        /// <summary>
        /// Met à jour un capteur existant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur à mettre à jour.</param>
        /// <param name="sensorDto">Les nouveaux détails du capteur.</param>
        /// <returns>Aucune réponse si la mise à jour est réussie.</returns>
        /// <response code="204">Mise à jour réussie.</response>
        /// <response code="404">Capteur non trouvé.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CapteurDto sensorDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _sensorService.UpdateCapteurAsync(id, sensorDto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Supprime un capteur par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du capteur à supprimer.</param>
        /// <returns>Aucune réponse si la suppression est réussie.</returns>
        /// <response code="204">Suppression réussie.</response>
        /// <response code="404">Capteur non trouvé.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid sensor id provided.");
            }

            try
            {
                var deleted = await _sensorService.DeleteCapteurAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}