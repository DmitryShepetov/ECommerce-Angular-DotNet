using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using System.Text.RegularExpressions;

namespace HoneyShop.Services
{
    public class OrderStatusHistoryService : IOrderStatusHistoryService
    {
        private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
        public OrderStatusHistoryService(IOrderStatusHistoryRepository orderStatusHistoryRepository)
        {
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
        }
        public async Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryStatusByIdAsync(int idOrder)
        {
            var orders = await _orderStatusHistoryRepository.GetByIdOrderAsync(idOrder);
            if (orders == null)
            {
                throw new KeyNotFoundException($"Order with id Order {idOrder} not found.");
            }
            return orders.Select(p => new OrderStatusHistoryDto
            {
                id = p.id,
                status = p.status,
                changedAt = p.changedAt,
                OrderId = p.OrderId
            });
        }
        public async Task AddOrderAsync(OrderStatusHistoryDto orderStatusDto)
        {
            ValidateOrderHistoryDto(orderStatusDto);
            // Преобразование DTO в сущность
            var order = new OrderStatusHistory
            {
                status = orderStatusDto.status,
                changedAt = orderStatusDto.changedAt,
                OrderId = orderStatusDto.OrderId
            };

            // Сохранение через репозиторий
            await _orderStatusHistoryRepository.AddAsync(order);
        }
        public async Task DeleteOrderAsync(int id)
        {
            // Проверяем, существует ли продукт
            var order = await _orderStatusHistoryRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order status with ID {id} not found.");
            }

            // Удаляем продукт
            await _orderStatusHistoryRepository.DeleteAsync(id);
        }
        public async Task UpdateOrderAsync(int id, OrderStatusHistoryDto orderStatusHistoryDto)
        {
            ValidateOrderHistoryDto(orderStatusHistoryDto);
            var existingProduct = await _orderStatusHistoryRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Watch with ID {id} not found.");
            }
            existingProduct.status = orderStatusHistoryDto.status ?? existingProduct.status;
            existingProduct.changedAt = orderStatusHistoryDto.changedAt;
            existingProduct.OrderId = orderStatusHistoryDto.OrderId;

            await _orderStatusHistoryRepository.UpdateAsync(existingProduct);
        }
        private static void ValidateOrderHistoryDto(OrderStatusHistoryDto orderStatusDto)
        {
            if (string.IsNullOrWhiteSpace(orderStatusDto.status) || orderStatusDto.status.Length < 2 || orderStatusDto.status.Length > 50)
                throw new ArgumentException("Статус должен быть от 2 до 50 символов.");
        }
    }
}
