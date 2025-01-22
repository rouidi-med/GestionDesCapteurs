namespace Capteurs.Entities
{
    public class Capteur
    {
        /// <summary>
        /// L'ID du capteur.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du capteur.
        /// </summary>
        public string Nom { get; set; }

        /// <summary>
        /// Description du capteur.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indique si capteur est active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
