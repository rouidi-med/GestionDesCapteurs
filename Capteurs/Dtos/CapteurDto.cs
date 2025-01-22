using System.ComponentModel.DataAnnotations;

namespace Capteurs.Dtos
{
    public class CapteurDto
    {
        /// <summary>
        /// L'ID du capteur.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du capteur.
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Nom { get; set; }

        /// <summary>
        /// Description du capteur.
        /// </summary>
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 100 caractères.")]
        public string Description { get; set; }
    }
}
