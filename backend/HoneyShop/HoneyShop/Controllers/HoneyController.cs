using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;

using static System.Net.Mime.MediaTypeNames;

namespace HoneyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoneyController : ControllerBase
    {
        private readonly IHoneyService _honeyService;
        public HoneyController(IHoneyService honeyService)
        {
            _honeyService = honeyService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHoney(CancellationToken cancellationToken)
        {
            try
            {
                var honey = await _honeyService.GetAllHoneyAsync(cancellationToken);
                return Ok(honey);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ошибка сервера", Error = ex.Message });
            }
        }
        [HttpGet("withCategory")]
        public async Task<IActionResult> GetAllHoneyWithCategory(CancellationToken cancellationToken)
        {
            try
            {
                var honey = await _honeyService.GetAllHoneyWithCategoryAsync(cancellationToken);
                return Ok(honey);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ошибка сервера", Error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHoneyById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var honey = await _honeyService.GetHoneyByIdAsync(id, cancellationToken);
                return Ok(honey);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddHoney([FromBody] HoneyDto honeyDto, CancellationToken cancellationToken)
        {
            try
            {
                await _honeyService.AddHoneyAsync(honeyDto, cancellationToken);
                return Ok(new { message = "Product added successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoney(int id, [FromBody] HoneyDto honeyDto, CancellationToken cancellationToken)
        {
            try
            {
                await _honeyService.UpdateHoneyAsync(id, honeyDto, cancellationToken);
                return Ok(new { message = "Product updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoney(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _honeyService.DeleteHoneyAsync(id, cancellationToken);
                return Ok(new { message = "Product deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Файл не выбран" });
            }

            // Проверяем допустимые форматы (только PNG и JPG)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { Message = "Допустимы только файлы JPG и PNG" });
            }

            // Проверяем размеры изображения
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream(), cancellationToken)) 
            {
                if (image.Width < 800 || image.Height < 800)
                {
                    return BadRequest(new { Message = "Изображение должно быть не менее 800x800 пикселей" });
                }
            }

            // Создаем папку uploads, если её нет
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            string imageUrl = $"/uploads/{fileName}";

            return Ok(new { url = imageUrl });
        }
    }
}
