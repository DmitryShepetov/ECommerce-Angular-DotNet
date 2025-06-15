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
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapToDto);
        }
        public async Task<IEnumerable<OrderDto>> GetOrderByPhoneNumberAsync(string phone)
        {
            var orders = await _orderRepository.GetOrderByPhoneNumberAsync(phone);
            if (orders == null)
            {
                throw new KeyNotFoundException($"Order with phone {phone} not found.");
            }
            return orders.Select(MapToDto);
        }
        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }
            return MapToDto(order);
        }
        public async Task AddOrderAsync(OrderDto orderDto)
        {
            ValidateOrderDto(orderDto);
            // Преобразование DTO в сущность
            var order = new Order
            {
                firstName = orderDto.firstName,
                lastName = orderDto.lastName,
                methodDeliveryData = orderDto.methodDeliveryData,
                deliveryMethod = orderDto.deliveryMethod,
                paymentMethod = orderDto.paymentMethod,
                adress = orderDto.adress,
                phone = orderDto.phone,
                email = orderDto.email,
                dateTime = orderDto.dateTime,
                totalPrice = orderDto.totalPrice,
                Items = orderDto.Items.Select(i => new OrderItem
                {
                    imageUrl = i.imageUrl,
                    name = i.name,
                    price = i.price,
                    quantity = i.quantity
                }).ToList()
            };

            // Сохранение через репозиторий
            await _orderRepository.AddAsync(order);
            var orderStatusHistory = new OrderStatusHistory
            {
                status = "Принят в обработку",
                changedAt = DateTime.Now,
                OrderId = order.id
            };
            await _orderStatusHistoryRepository.AddAsync(orderStatusHistory);
        }
        public async Task DeleteOrderAsync(int id)
        {
            // Проверяем, существует ли продукт
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            // Удаляем продукт
            await _orderRepository.DeleteAsync(id);
        }
        public async Task UpdateOrderAsync(int id, OrderDto orderDto)
        {
            ValidateOrderDto(orderDto);
            var existingProduct = await _orderRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Watch with ID {id} not found.");
            }
            existingProduct.firstName = orderDto.firstName ?? existingProduct.firstName;
            existingProduct.lastName = orderDto.lastName ?? existingProduct.lastName;
            existingProduct.methodDeliveryData = orderDto.methodDeliveryData ?? existingProduct.methodDeliveryData;
            existingProduct.deliveryMethod = orderDto.deliveryMethod ?? existingProduct.deliveryMethod;
            existingProduct.paymentMethod = orderDto.paymentMethod ?? existingProduct.paymentMethod;
            existingProduct.adress = orderDto.adress ?? existingProduct.adress;
            existingProduct.phone = orderDto.phone ?? existingProduct.phone;
            existingProduct.email = orderDto.email ?? existingProduct.email;
            existingProduct.dateTime = orderDto.dateTime;
            existingProduct.totalPrice = orderDto.totalPrice;
            existingProduct.Items = orderDto.Items.Select(i => new OrderItem
            {
                imageUrl = i.imageUrl,
                name = i.name,
                price = i.price,
                quantity = i.quantity
            }).ToList();

            await _orderRepository.UpdateAsync(existingProduct);
        }

        private static OrderDto MapToDto(Order p) => new OrderDto
        {
            id = p.id,
            firstName = p.firstName,
            lastName = p.lastName,
            methodDeliveryData = p.methodDeliveryData,
            deliveryMethod = p.deliveryMethod,
            paymentMethod = p.paymentMethod,
            adress = p.adress,
            phone = p.phone,
            email = p.email,
            dateTime = p.dateTime,
            totalPrice = p.totalPrice,
            Items = (p.Items ?? new List<OrderItem>()).Select(i => new OrderItemDto
            {
                imageUrl = i.imageUrl,
                name = i.name,
                price = i.price,
                quantity = i.quantity
            }).ToList()
        };

        private static void ValidateOrderDto(OrderDto orderDto)
        {
            if (string.IsNullOrWhiteSpace(orderDto.firstName) || orderDto.firstName.Length < 2 || orderDto.firstName.Length > 30)
                throw new ArgumentException("Имя должно быть от 2 до 30 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.lastName) || orderDto.lastName.Length < 2 || orderDto.lastName.Length > 30)
                throw new ArgumentException("Фамилия должна быть от 2 до 30 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.methodDeliveryData))
                throw new ArgumentException("Метод времени доставки не может быть пустым.");

            if (string.IsNullOrWhiteSpace(orderDto.deliveryMethod) || orderDto.deliveryMethod.Length < 5 || orderDto.deliveryMethod.Length > 25)
                throw new ArgumentException("Метод доставки должен быть от 5 до 25 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.paymentMethod) || orderDto.paymentMethod.Length < 5 || orderDto.paymentMethod.Length > 50)
                throw new ArgumentException("Метод оплаты должен быть от 5 до 50 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.adress) || orderDto.adress.Length < 10 || orderDto.adress.Length > 150)
                throw new ArgumentException("Адрес должен быть от 10 до 150 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.phone) || !Regex.IsMatch(orderDto.phone, @"^\+?\d{6,15}$"))
                throw new ArgumentException("Номер телефона должен быть от 6 до 15 символов.");

            if (string.IsNullOrWhiteSpace(orderDto.email) || !Regex.IsMatch(orderDto.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || orderDto.email.Length < 10 || orderDto.email.Length > 150)
                throw new ArgumentException("Email должен быть от 5 до 50 символов.");

            if (orderDto.totalPrice <= 0)
                throw new ArgumentException("Общая стоимость должна быть больше 0.");
        }
    }
}
