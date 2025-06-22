using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoneyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Server error", Error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id, cancellationToken);
                return Ok(category);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.AddCategoryAsync(categoryDto, cancellationToken);
                return Ok(new { message = "Category added successfully." });
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
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.UpdateCategoryAsync(id, categoryDto, cancellationToken);
                return Ok("Category updated successfully.");
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id, cancellationToken);
                return Ok(new { message = "Category deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
