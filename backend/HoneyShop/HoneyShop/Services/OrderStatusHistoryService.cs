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
        public async Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryStatusByIdAsync(int idOrder, CancellationToken cancellationToken = default)
        {
            var orders = await _orderStatusHistoryRepository.GetByIdOrderAsync(idOrder, cancellationToken);
            if (orders == null)
            {
                throw new KeyNotFoundException($"Order with id Order {idOrder} not found.");
            }
            return orders.Select(p => new OrderStatusHistoryDto
            {
                Id = p.Id,
                Status = p.Status,
                ChangedAt = p.ChangedAt,
                OrderId = p.OrderId
            });
        }
        public async Task AddOrderAsync(OrderStatusHistoryDto orderStatusDto, CancellationToken cancellationToken = default)
        {
            ValidateOrderHistoryDto(orderStatusDto);
            // Преобразование DTO в сущность
            var order = new OrderStatusHistory
            {
                Status = orderStatusDto.Status,
                ChangedAt = orderStatusDto.ChangedAt,
                OrderId = orderStatusDto.OrderId
            };

            // Сохранение через репозиторий
            await _orderStatusHistoryRepository.AddAsync(order, cancellationToken);
        }
        public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default)
        {
            // Проверяем, существует ли продукт
            var order = await _orderStatusHistoryRepository.GetByIdAsync(id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order status with ID {id} not found.");
            }

            // Удаляем продукт
            await _orderStatusHistoryRepository.DeleteAsync(id, cancellationToken);
        }
        public async Task UpdateOrderAsync(int id, OrderStatusHistoryDto orderStatusHistoryDto, CancellationToken cancellationToken = default)
        {
            ValidateOrderHistoryDto(orderStatusHistoryDto);
            var existingProduct = await _orderStatusHistoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Watch with ID {id} not found.");
            }
            existingProduct.Status = orderStatusHistoryDto.Status ?? existingProduct.Status;
            existingProduct.ChangedAt = orderStatusHistoryDto.ChangedAt;
            existingProduct.OrderId = orderStatusHistoryDto.OrderId;

            await _orderStatusHistoryRepository.UpdateAsync(existingProduct, cancellationToken);
        }
        private static void ValidateOrderHistoryDto(OrderStatusHistoryDto orderStatusDto)
        {
            if (orderStatusDto == null)
                throw new ArgumentNullException(nameof(orderStatusDto));

            if (string.IsNullOrWhiteSpace(orderStatusDto.Status) || orderStatusDto.Status.Length < 2 || orderStatusDto.Status.Length > 50)
                throw new ArgumentException("Статус должен быть от 2 до 50 символов.");
        }
    }
}
