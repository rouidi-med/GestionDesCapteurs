namespace Capteurs.Constants
{
    /// <summary>
    /// Contient des constantes représentant les codes d'erreur utilisés dans l'application.
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>
        /// Code d'erreur indiquant qu'un capteur est introuvable.
        /// </summary>
        public const string SensorNotFound = "Err-0001";

        /// <summary>
        /// Code d'erreur indiquant que l'identifiant du capteur fourni est invalide.
        /// </summary>
        public const string InvalidSensorId = "Err-0002";

        /// <summary>
        /// Code d'erreur indiquant qu'une erreur interne au serveur s'est produite.
        /// </summary>
        public const string InternalServerError = "Err-0005";

        /// <summary>
        /// Code d'erreur indiquant que les données du capteur fournies sont invalides.
        /// </summary>
        public const string InvalidSensorData = "Err-0006";
    }
}
