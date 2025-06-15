using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;

namespace HoneyShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHoneyRepository _honeyRepository;
        public CategoryService(ICategoryRepository categoryRepository, IHoneyRepository honeyRepository)
        {
            _categoryRepository = categoryRepository;
            _honeyRepository = honeyRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var category = await _categoryRepository.GetAllAsync();
            return category.Select(p => new CategoryDto
            {
                id = p.id,
                categoryName = p.categoryName,
                desc = p.desc,
            });
        }
        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return new CategoryDto
            {
                id = category.id,
                categoryName = category.categoryName,
                desc = category.desc
            };
        }
        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            ValidateCategoryDto(categoryDto);
            // Преобразование DTO в сущность
            var category = new Category
            {
                categoryName = categoryDto.categoryName,
                desc = categoryDto.desc
            };

            // Сохранение через репозиторий
            await _categoryRepository.AddAsync(category);
        }
        public async Task DeleteCategoryAsync(int id)
        {
            // Проверяем, существует ли продукт
            var product = await _categoryRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            // Удаляем продукт
            await _categoryRepository.DeleteAsync(id);
        }
        public async Task UpdateProductAsync(int id, CategoryDto categoryDto)
        {
            ValidateCategoryDto(categoryDto);
            // Получаем существующий продукт
            var existingProduct = await _categoryRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }

            // Обновляем данные
            existingProduct.categoryName = categoryDto.categoryName;
            existingProduct.desc = categoryDto.desc;

            // Сохраняем изменения
            await _categoryRepository.UpdateAsync(existingProduct);
        }
        private static void ValidateCategoryDto(CategoryDto categoryDto)
        {
            if (string.IsNullOrWhiteSpace(categoryDto.categoryName) || categoryDto.categoryName.Length < 2 || categoryDto.categoryName.Length > 50)
                throw new ArgumentException("Название категории должно быть от 2 до 50 символов.");

            if (string.IsNullOrWhiteSpace(categoryDto.desc) || categoryDto.desc.Length > 500)
                throw new ArgumentException("Короткое описание не должно быть пустым и содержать не более 500 символов.");
        }
    }
}
