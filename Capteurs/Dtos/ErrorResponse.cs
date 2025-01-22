namespace Capteurs.Dtos
{
    public class ErrorResponse
    {
        /// <summary>
        /// Code d'erreur personnalisé
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Message explicite pour l'erreur
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Détails supplémentaires sur l'erreur/exception
        /// </summary>
        public string Details { get; set; }
    }
}
