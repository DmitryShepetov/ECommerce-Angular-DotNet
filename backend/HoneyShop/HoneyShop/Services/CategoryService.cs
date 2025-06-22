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

        public CategoryService(ICategoryRepository categoryRepository, IHoneyRepository honeyRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetAllAsync(cancellationToken);
            return category.Select(p => new CategoryDto
            {
                Id = p.Id,
                CategoryName = p.CategoryName,
                Desc = p.Desc,
            });
        }
        public async Task<CategoryDto> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Desc = category.Desc
            };
        }
        public async Task AddCategoryAsync(CategoryDto categoryDto, CancellationToken cancellationToken = default)
        {
            ValidateCategoryDto(categoryDto);
            // Преобразование DTO в сущность
            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Desc = categoryDto.Desc
            };

            // Сохранение через репозиторий
            await _categoryRepository.AddAsync(category, cancellationToken);
        }
        public async Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            // Проверяем, существует ли продукт
            var product = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            // Удаляем продукт
            await _categoryRepository.DeleteAsync(id, cancellationToken);
        }
        public async Task UpdateCategoryAsync(int id, CategoryDto categoryDto, CancellationToken cancellationToken = default)
        {
            ValidateCategoryDto(categoryDto);
            // Получаем существующий продукт
            var existingCategory = await _categoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            // Обновляем данные
            existingCategory.CategoryName = categoryDto.CategoryName;
            existingCategory.Desc = categoryDto.Desc;

            // Сохраняем изменения
            await _categoryRepository.UpdateAsync(existingCategory, cancellationToken);
        }
        private static void ValidateCategoryDto(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            if (string.IsNullOrWhiteSpace(categoryDto.CategoryName) || categoryDto.CategoryName.Length < 2 || categoryDto.CategoryName.Length > 50)
                throw new ArgumentException("Название категории должно быть от 2 до 50 символов.");

            if (string.IsNullOrWhiteSpace(categoryDto.Desc) || categoryDto.Desc.Length > 500)
                throw new ArgumentException("Короткое описание не должно быть пустым и содержать не более 500 символов.");
        }
    }
}
