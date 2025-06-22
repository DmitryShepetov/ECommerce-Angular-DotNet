using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;

namespace HoneyShop.Services
{
    public class HoneyService : IHoneyService
    {
        private readonly IHoneyRepository _honeyRepository;
        private readonly ICategoryRepository _categoryRepository;
        public HoneyService(IHoneyRepository honeyRepository, ICategoryRepository categoryRepository)
        {
            _honeyRepository = honeyRepository;
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<HoneyDto>> GetAllHoneyAsync(CancellationToken cancellationToken = default)
        {
            var honey = await _honeyRepository.GetAllAsync(cancellationToken);
            return honey.Select(MapToDto);
        }
        public async Task<IEnumerable<HoneyCategoryDto>> GetAllHoneyWithCategoryAsync(CancellationToken cancellationToken = default)
        {
            var honey = await _honeyRepository.GetAllWithCategoryAsync(cancellationToken);
            return honey.Select(p => new HoneyCategoryDto
            {
                Id = p.Id,
                Name = p.Name,
                ShortDesc = p.ShortDesc,
                LongDesc = p.LongDesc,
                ShelfLife = p.ShelfLife,
                IsFavorite = p.IsFavorite,
                Available = p.Available,
                NewHoney = p.NewHoney,
                Bju = p.Bju,
                PriceType = p.PriceType,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Categories = new CategoryDto
                {
                    Id = p.Category.Id,
                    CategoryName = p.Category.CategoryName,
                    Desc = p.Category.Desc
                }
            });
        }
        public async Task<HoneyDto> GetHoneyByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var honey = await _honeyRepository.GetByIdAsync(id, cancellationToken);
            if (honey == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return MapToDto(honey);
        }
        public async Task AddHoneyAsync(HoneyDto honeyDto, CancellationToken cancellationToken = default)
        {
            ValidateHoneyDto(honeyDto);
            var category = await _categoryRepository.GetByIdAsync(honeyDto.CategoryId, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with ID {honeyDto.CategoryId} not found.");

            var honey = new Honey
            {
                Name = honeyDto.Name,
                ShortDesc = honeyDto.ShortDesc,
                LongDesc = honeyDto.LongDesc,
                ShelfLife = honeyDto.ShelfLife,
                IsFavorite = honeyDto.IsFavorite,
                Available = honeyDto.Available,
                NewHoney = honeyDto.NewHoney,
                Bju = honeyDto.Bju,
                PriceType = honeyDto.PriceType,
                Price = honeyDto.Price,
                ImageUrl = honeyDto.ImageUrl,
                CategoryId = honeyDto.CategoryId
            };

            // Сохранение через репозиторий
            await _honeyRepository.AddAsync(honey, cancellationToken);
        }
        public async Task DeleteHoneyAsync(int id, CancellationToken cancellationToken = default)
        {
            // Проверяем, существует ли продукт
            var honey = await _honeyRepository.GetByIdAsync(id, cancellationToken);
            if (honey == null)
            {
                throw new KeyNotFoundException($"Honey with ID {id} not found.");
            }

            // Удаляем продукт
            await _honeyRepository.DeleteAsync(id, cancellationToken);
        }
        public async Task UpdateHoneyAsync(int id, HoneyDto honeyDto, CancellationToken cancellationToken = default)
        {
            ValidateHoneyDto(honeyDto);
            var existingProduct = await _honeyRepository.GetByIdAsync(id, cancellationToken);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Honey with ID {id} not found.");
            }

            var category = await _categoryRepository.GetByIdAsync(honeyDto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {honeyDto.CategoryId} not found.");
            }
            // Удаляем старое изображение, если оно было изменено
            if (!string.IsNullOrEmpty(honeyDto.ImageUrl) && honeyDto.ImageUrl != existingProduct.ImageUrl)
            {
                DeleteOldImage(existingProduct.ImageUrl);
            }

            existingProduct.Name = honeyDto.Name ?? existingProduct.Name;
            existingProduct.ShortDesc = honeyDto.ShortDesc ?? existingProduct.ShortDesc;
            existingProduct.LongDesc = honeyDto.LongDesc ?? existingProduct.LongDesc;
            existingProduct.ShelfLife = honeyDto.ShelfLife ?? existingProduct.ShelfLife;
            existingProduct.IsFavorite = honeyDto.IsFavorite;
            existingProduct.Available = honeyDto.Available;
            existingProduct.NewHoney = honeyDto.NewHoney;
            existingProduct.Bju = honeyDto.Bju ?? existingProduct.Bju;
            existingProduct.PriceType = honeyDto.PriceType;
            existingProduct.Price = honeyDto.Price;
            existingProduct.ImageUrl = honeyDto.ImageUrl;
            existingProduct.CategoryId = honeyDto.CategoryId;

            await _honeyRepository.UpdateAsync(existingProduct, cancellationToken);
        }

        private static HoneyDto MapToDto(Honey honey) => new HoneyDto
        {
            Id = honey.Id,
            Name = honey.Name,
            ShortDesc = honey.ShortDesc,
            LongDesc = honey.LongDesc,
            ShelfLife = honey.ShelfLife,
            IsFavorite = honey.IsFavorite,
            Available = honey.Available,
            NewHoney = honey.NewHoney,
            Bju = honey.Bju,
            PriceType = honey.PriceType,
            Price = honey.Price,
            ImageUrl = honey.ImageUrl,
            CategoryId = honey.CategoryId
        };

        private void DeleteOldImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
        }

        private static void ValidateHoneyDto(HoneyDto honeyDto)
        {
            if (honeyDto == null)
                throw new ArgumentNullException(nameof(honeyDto));

            if (string.IsNullOrWhiteSpace(honeyDto.Name) || honeyDto.Name.Length < 2 || honeyDto.Name.Length > 100)
                throw new ArgumentException("Название мёда должно быть от 2 до 100 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.ShortDesc) || honeyDto.ShortDesc.Length < 5 || honeyDto.ShortDesc.Length > 300)
                throw new ArgumentException("Короткое описание должно быть от 5 до 300 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.LongDesc) || honeyDto.LongDesc.Length < 10)
                throw new ArgumentException("Полное описание должно быть минимум 10 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.ShelfLife) || honeyDto.ShelfLife.Length > 50)
                throw new ArgumentException("Срок хранения не может быть пустой и не более 50 символов.");

            if (honeyDto.Price <= 0)
                throw new ArgumentException("Цена должна быть больше 0.");

            if (string.IsNullOrWhiteSpace(honeyDto.Bju))
                throw new ArgumentException("БЖУ не может быть пустым.");

            if (string.IsNullOrWhiteSpace(honeyDto.PriceType))
                throw new ArgumentException("Тип цены не может быть пустым.");

            if (string.IsNullOrWhiteSpace(honeyDto.ImageUrl))
                throw new ArgumentException("Ссылка на изображение не может быть пустой.");
        }
    }
}
