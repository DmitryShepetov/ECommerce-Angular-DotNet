using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using System.Text.RegularExpressions;

namespace HoneyShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
        public OrderService(IOrderRepository orderRepository, IOrderStatusHistoryRepository orderStatusHistoryRepository)
        {
            _orderRepository = orderRepository;
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
        }
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            return orders.Select(MapToDto);
        }
        public async Task<IEnumerable<OrderDto>> GetOrderByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.GetOrderByPhoneAsync(phone, cancellationToken);
            if (orders == null)
            {
                throw new KeyNotFoundException($"Order with phone {phone} not found.");
            }
            return orders.Select(MapToDto);
        }
        public async Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            return MapToDto(order);
        }

        public async Task<(List<OrderDto> Orders, int TotalCount)> GetPaginatedOrdersAsync(PaginationParameters paginationParams, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetPaginatedOrdersAsync(paginationParams, cancellationToken);
            var total = await _orderRepository.GetTotalOrdersCountAsync(cancellationToken);

            var orderDtos = orders.Select(MapToDto).ToList();
            return (orderDtos, total);
        }

        public async Task AddOrderAsync(OrderDto orderDto, CancellationToken cancellationToken = default)
        {
            ValidateOrderDto(orderDto);
            // Преобразование DTO в сущность
            var order = new Order
            {
                FirstName = orderDto.FirstName,
                LastName = orderDto.LastName,
                MethodDeliveryData = orderDto.MethodDeliveryData,
                DeliveryMethod = orderDto.DeliveryMethod,
                PaymentMethod = orderDto.PaymentMethod,
                Adress = orderDto.Adress,
                Phone = orderDto.Phone,
                Email = orderDto.Email,
                DateTime = orderDto.DateTime,
                TotalPrice = orderDto.TotalPrice,
                Items = orderDto.Items.Select(i => new OrderItem
                {
                    ImageUrl = i.ImageUrl,
                    Name = i.Name,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Сохранение через репозиторий
            await _orderRepository.AddAsync(order, cancellationToken);
            var orderStatusHistory = new OrderStatusHistory
            {
                Status = "Принят в обработку",
                ChangedAt = DateTime.Now,
                OrderId = order.Id
            };
            await _orderStatusHistoryRepository.AddAsync(orderStatusHistory, cancellationToken);
        }
        public async Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default)
        {
            // Проверяем, существует ли продукт
            var order = await _orderRepository.GetOrderByIdAsync(id, cancellationToken);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            // Удаляем продукт
            await _orderRepository.DeleteAsync(id, cancellationToken);
        }
        public async Task UpdateOrderAsync(int id, OrderDto orderDto, CancellationToken cancellationToken = default)
        {
            ValidateOrderDto(orderDto);
            var existingProduct = await _orderRepository.GetOrderByIdAsync(id, cancellationToken);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Watch with ID {id} not found.");
            }
            existingProduct.FirstName = orderDto.FirstName ?? existingProduct.FirstName;
            existingProduct.LastName = orderDto.LastName ?? existingProduct.LastName;
            existingProduct.MethodDeliveryData = orderDto.MethodDeliveryData ?? existingProduct.MethodDeliveryData;
            existingProduct.DeliveryMethod = orderDto.DeliveryMethod ?? existingProduct.DeliveryMethod;
            existingProduct.PaymentMethod = orderDto.PaymentMethod ?? existingProduct.PaymentMethod;
            existingProduct.Adress = orderDto.Adress ?? existingProduct.Adress;
            existingProduct.Phone = orderDto.Phone ?? existingProduct.Phone;
            existingProduct.Email = orderDto.Email ?? existingProduct.Email;
            existingProduct.DateTime = orderDto.DateTime;
            existingProduct.TotalPrice = orderDto.TotalPrice;
            existingProduct.Items = orderDto.Items.Select(i => new OrderItem
            {
                ImageUrl = i.ImageUrl,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList();

            await _orderRepository.UpdateAsync(existingProduct, cancellationToken);
        }

        private static OrderDto MapToDto(Order p) => new OrderDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            MethodDeliveryData = p.MethodDeliveryData,
            DeliveryMethod = p.DeliveryMethod,
            PaymentMethod = p.PaymentMethod,
            Adress = p.Adress,
            Phone = p.Phone,
            Email = p.Email,
            DateTime = p.DateTime,
            TotalPrice = p.TotalPrice,
            Items = (p.Items ?? new List<OrderItem>()).Select(i => new OrderItemDto
            {
                ImageUrl = i.ImageUrl,
                Name = i.Name,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        private static void ValidateOrderDto(OrderDto orderDto)
        {
            if (orderDto == null)
                throw new ArgumentNullException(nameof(orderDto));

            if (string.IsNullOrWhiteSpace(orderDto.FirstName) || orderDto.FirstName.Length < 2 || orderDto.FirstName.Length > 30)
                throw new ArgumentException("Имя должно быть от 2 до 30 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.LastName) || orderDto.LastName.Length < 2 || orderDto.LastName.Length > 30)
                throw new ArgumentException("Фамилия должна быть от 2 до 30 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.MethodDeliveryData))
                throw new ArgumentException("Метод времени доставки не может быть пустым.");

            if (string.IsNullOrWhiteSpace(orderDto.DeliveryMethod) || orderDto.DeliveryMethod.Length < 5 || orderDto.DeliveryMethod.Length > 25)
                throw new ArgumentException("Метод доставки должен быть от 5 до 25 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.PaymentMethod) || orderDto.PaymentMethod.Length < 5 || orderDto.PaymentMethod.Length > 50)
                throw new ArgumentException("Метод оплаты должен быть от 5 до 50 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.Adress) || orderDto.Adress.Length < 10 || orderDto.Adress.Length > 150)
                throw new ArgumentException("Адрес должен быть от 10 до 150 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.Phone) || !Regex.IsMatch(orderDto.Phone, @"^\+?\d{6,15}$"))
                throw new ArgumentException("Номер телефона должен быть от 6 до 15 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.Email) || !Regex.IsMatch(orderDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || orderDto.Email.Length < 10 || orderDto.Email.Length > 150)
                throw new ArgumentException("Email должен быть от 5 до 50 символов.");

            if (orderDto.TotalPrice <= 0)
                throw new ArgumentException("Общая стоимость должна быть больше 0.");
        }
    }
}
