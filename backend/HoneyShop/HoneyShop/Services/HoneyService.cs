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
        public async Task<IEnumerable<HoneyDto>> GetAllHoneyAsync()
        {
            var honey = await _honeyRepository.GetAllAsync();
            return honey.Select(MapToDto);
        }
        public async Task<IEnumerable<HoneyCategoryDto>> GetAllHoneyWithCategoryAsync()
        {
            var honey = await _honeyRepository.GetAllWithCategoryAsync();
            return honey.Select(p => new HoneyCategoryDto
            {
                id = p.id,
                name = p.name,
                shortDesc = p.shortDesc,
                longDesc = p.longDesc,
                shelfLife = p.shelfLife,
                isFavorite = p.isFavorite,
                avaliable = p.avaliable,
                newHoney = p.newHoney,
                bju = p.bju,
                priceType = p.priceType,
                price = p.price,
                imageUrl = p.imageUrl,
                Categories = new CategoryDto
                {
                    id = p.Category.id,
                    categoryName = p.Category.categoryName,
                    desc = p.Category.desc
                }
            });
        }
        public async Task<HoneyDto> GetHoneyByIdAsync(int id)
        {
            var honey = await _honeyRepository.GetByIdAsync(id);
            if (honey == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            return MapToDto(honey);
        }
        public async Task AddHoneyAsync(HoneyDto honeyDto)
        {
            ValidateHoneyDto(honeyDto);
            var category = await _categoryRepository.GetByIdAsync(honeyDto.Categoryid)
            ?? throw new KeyNotFoundException($"Category with ID {honeyDto.Categoryid} not found.");

            var honey = new Honey
            {
                name = honeyDto.name,
                shortDesc = honeyDto.shortDesc,
                longDesc = honeyDto.longDesc,
                shelfLife = honeyDto.shelfLife,
                isFavorite = honeyDto.isFavorite,
                avaliable = honeyDto.avaliable,
                newHoney = honeyDto.newHoney,
                bju = honeyDto.bju,
                priceType = honeyDto.priceType,
                price = honeyDto.price,
                imageUrl = honeyDto.imageUrl,
                Categoryid = honeyDto.Categoryid
            };

            // Сохранение через репозиторий
            await _honeyRepository.AddAsync(honey);
        }
        public async Task DeleteHoneyAsync(int id)
        {
            // Проверяем, существует ли продукт
            var honey = await _honeyRepository.GetByIdAsync(id);
            if (honey == null)
            {
                throw new KeyNotFoundException($"Honey with ID {id} not found.");
            }

            // Удаляем продукт
            await _honeyRepository.DeleteAsync(id);
        }
        public async Task UpdateHoneyAsync(int id, HoneyDto honeyDto)
        {
            ValidateHoneyDto(honeyDto);
            var existingProduct = await _honeyRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Honey with ID {id} not found.");
            }

            var category = await _categoryRepository.GetByIdAsync(honeyDto.Categoryid);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {honeyDto.Categoryid} not found.");
            }
            // Удаляем старое изображение, если оно было изменено
            if (!string.IsNullOrEmpty(honeyDto.imageUrl) && honeyDto.imageUrl != existingProduct.imageUrl)
            {
                DeleteOldImage(existingProduct.imageUrl);
            }

            existingProduct.name = honeyDto.name ?? existingProduct.name;
            existingProduct.shortDesc = honeyDto.shortDesc ?? existingProduct.shortDesc;
            existingProduct.longDesc = honeyDto.longDesc ?? existingProduct.longDesc;
            existingProduct.shelfLife = honeyDto.shelfLife ?? existingProduct.shelfLife;
            existingProduct.isFavorite = honeyDto.isFavorite;
            existingProduct.avaliable = honeyDto.avaliable;
            existingProduct.newHoney = honeyDto.newHoney;
            existingProduct.bju = honeyDto.bju ?? existingProduct.bju;
            existingProduct.priceType = honeyDto.priceType;
            existingProduct.price = honeyDto.price;
            existingProduct.imageUrl = honeyDto.imageUrl;
            existingProduct.Categoryid = honeyDto.Categoryid;

            await _honeyRepository.UpdateAsync(existingProduct);
        }

        private static HoneyDto MapToDto(Honey honey) => new HoneyDto
        {
            id = honey.id,
            name = honey.name,
            shortDesc = honey.shortDesc,
            longDesc = honey.longDesc,
            shelfLife = honey.shelfLife,
            isFavorite = honey.isFavorite,
            avaliable = honey.avaliable,
            newHoney = honey.newHoney,
            bju = honey.bju,
            priceType = honey.priceType,
            price = honey.price,
            imageUrl = honey.imageUrl,
            Categoryid = honey.Categoryid
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
            if (string.IsNullOrWhiteSpace(honeyDto.name) || honeyDto.name.Length < 2 || honeyDto.name.Length > 100)
                throw new ArgumentException("Название мёда должно быть от 2 до 100 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.shortDesc) || honeyDto.shortDesc.Length < 5 || honeyDto.shortDesc.Length > 300)
                throw new ArgumentException("Короткое описание должно быть от 5 до 300 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.longDesc) || honeyDto.longDesc.Length < 10)
                throw new ArgumentException("Полное описание должно быть минимум 10 символов.");

            if (string.IsNullOrWhiteSpace(honeyDto.shelfLife) || honeyDto.shelfLife.Length > 50)
                throw new ArgumentException("Срок хранения не может быть пустой и не более 50 символов.");

            if (honeyDto.price <= 0)
                throw new ArgumentException("Цена должна быть больше 0.");

            if (string.IsNullOrWhiteSpace(honeyDto.bju))
                throw new ArgumentException("БЖУ не может быть пустым.");

            if (string.IsNullOrWhiteSpace(honeyDto.priceType))
                throw new ArgumentException("Тип цены не может быть пустым.");

            if (string.IsNullOrWhiteSpace(honeyDto.imageUrl))
                throw new ArgumentException("Ссылка на изображение не может быть пустой.");
        }
    }
}
