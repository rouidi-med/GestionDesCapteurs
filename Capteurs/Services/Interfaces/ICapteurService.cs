using Capteurs.Dtos;

namespace Capteurs.Services.Interfaces
{
    public interface ICapteurService
    {
        Task<IEnumerable<CapteurDto>> GetAllCapteursAsync();
        Task<CapteurDto> GetCapteurByIdAsync(int id);
        Task<CapteurDto> AddCapteurAsync(CapteurDto capteurDto);
        Task<bool> UpdateCapteurAsync(int id, CapteurDto capteurDto);
        Task<bool> DeleteCapteurAsync(int id);

        //v2 methods
        Task<bool> ArchiveCapteurAsync(int id);
        Task<bool> RestoreCapteurAsync(int id);
    }
}
