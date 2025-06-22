using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
        Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateCategoryAsync(int id, CategoryDto categoryDto, CancellationToken cancellationToken = default);
    }
}
