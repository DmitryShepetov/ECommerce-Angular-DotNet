using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
        Task UpdateProductAsync(int id, CategoryDto categoryDto);
    }
}
