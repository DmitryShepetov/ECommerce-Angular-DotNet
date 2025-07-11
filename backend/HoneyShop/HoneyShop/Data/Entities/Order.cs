﻿using HoneyShop.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HoneyShop.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Имя")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Максимальная длина имени 30 символов")]
        [Required(ErrorMessage = "Напишите своё имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Максимальная длина фамилии 30 символов")]
        [Required(ErrorMessage = "Напишите свою фамилию")]
        public string LastName { get; set; }
        [Display(Name = "Метод доставки")]
        [Required(ErrorMessage = "Напишите Как можно скорее/Дата доставки")]
        public string MethodDeliveryData { get; set; }
        [Display(Name = "Доставка")]
        [StringLength(25, MinimumLength = 5, ErrorMessage = "Максимальная длина метода доставки 25 символов")]
        [Required(ErrorMessage = "Напишите способ доставки")]
        public string DeliveryMethod { get; set; }
        [Display(Name = "Способ оплаты")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Максимальная длина метода оплаты 50 символов")]
        [Required(ErrorMessage = "Напишите способ оплаты")]
        public string PaymentMethod { get; set; }
        [Display(Name = "Адрес")]
        [StringLength(150, MinimumLength = 10, ErrorMessage = "Максимальная длина адреса 150 символов")]
        [Required(ErrorMessage = "Напишите адрес")]
        public string Adress { get; set; }
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Напишите свой номер телефона")]
        [RegularExpression(@"^\+?\d{6,15}$", ErrorMessage = "Номер телефона должен содержать от 6 до 15 цифр и может начинаться с '+'")]
        public string Phone { get; set; }
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Максимальная длина email 50 символов")]
        [Required(ErrorMessage = "Email должен содержать от 5 до 50 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; }
        [Display(Name = "Цена")]
        [Range(1, 1000000, ErrorMessage = "Цена должна быть больше 0")]
        [Required(ErrorMessage = "Напишите цену")]
        public int TotalPrice { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<OrderStatusHistory> StatusHistory { get; set; }
        [Display(Name = "Дата")]
        public DateTime DateTime { get; set; }
    }
}
