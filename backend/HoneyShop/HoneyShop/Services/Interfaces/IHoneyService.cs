using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IHoneyService
    {
        Task<IEnumerable<HoneyDto>> GetAllHoneyAsync(CancellationToken cancellationToken = default);
        Task<HoneyDto> GetHoneyByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<HoneyCategoryDto>> GetAllHoneyWithCategoryAsync(CancellationToken cancellationToken = default);
        Task AddHoneyAsync(HoneyDto honeyDto, CancellationToken cancellationToken = default);
        Task DeleteHoneyAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateHoneyAsync(int id, HoneyDto honeyDto, CancellationToken cancellationToken = default);

    }
}
