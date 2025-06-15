using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IHoneyService
    {
        Task<IEnumerable<HoneyDto>> GetAllHoneyAsync();
        Task<HoneyDto> GetHoneyByIdAsync(int id);
        Task<IEnumerable<HoneyCategoryDto>> GetAllHoneyWithCategoryAsync();
        Task AddHoneyAsync(HoneyDto honeyDto);
        Task DeleteHoneyAsync(int id);
        Task UpdateHoneyAsync(int id, HoneyDto honeyDto);

    }
}
