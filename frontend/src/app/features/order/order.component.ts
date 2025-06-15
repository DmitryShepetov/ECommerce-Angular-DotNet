import { AfterViewInit, Component, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { Data, Router, RouterModule } from '@angular/router';
import { OrderService } from '../../core/services/order.service'; // Сервис для отправки данных в БД
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-order',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.scss']
})

export class OrderComponent implements OnInit {
  cart: any[] = [];
  name: string = '';
  surname: string = '';
  phone: string = '';
  address: string = 'г. Быхов, ул. Молодежная, д. 32';
  email: string = '';
  deliveryMethod: string = 'курьер';
  methodDeliveryData: string = 'как можно скорее';
  paymentMethod: string = 'наличными';
  totalPrice: number = 0;
  selectedOption: string = 'курьер';
  selectedPayment: string = 'наличными';
  selectedData: string = 'как можно скорее';
  selectedAddress: string = '1';
  dateTime: string | null = null;


  

  minDate: string = '';
  maxDate: string = '';
  isDelivery = false;
  isContact = true;
  isPayment = false;

  constructor(private cartService: CartService, private orderService: OrderService, private router: Router, private http: HttpClient) {}

   ngOnInit() {
    this.cart = this.cartService.getCart();
    this.totalPrice = this.cart.reduce((total, item) => total + item.price * item.quantity, 0);
    this.updateAddress();
    this.setDateLimits();
  }


  updateAddress(): void {
    if (this.selectedOption === 'курьер') {
      this.address = ''; // Очищаем, чтобы пользователь вводил вручную
    } else if (this.selectedOption === 'самовывоз' && this.selectedAddress === '') {
      this.selectedAddress = 'г. Быхов, ул. Молодежная, д. 32'; // Если не выбран адрес, устанавливаем его автоматически
      this.address = this.selectedAddress;
    }
  }
  
  // Вызываем этот метод при выборе адреса самовывоза
  setPickupAddress(option: string): void {
    this.selectedAddress = option;
    this.address = option;
  }

  setDeliveryMethod(option: string): void {
    this.selectedOption = option;
    this.deliveryMethod = option;
  }
  setDeliveryMethodData(option: string): void {
    this.selectedData = option;
    this.methodDeliveryData = option;
  }

  setPaymentMethod(option: string): void{
    this.selectedPayment = option;
    this.paymentMethod = option;
  }

  submitOrder(): void {
    if (!this.dateTime) {
      this.dateTime = null; // Убедимся, что пустая строка не отправляется
    }
    const orderData = {
      firstName: this.name,
      lastName: this.surname,
      phone: this.phone,
      email: this.email,
      adress: this.address,
      methodDeliveryData: this.methodDeliveryData,
      deliveryMethod: this.deliveryMethod,
      paymentMethod: this.paymentMethod,
      Items: this.cart.map(item => ({
        id: item.id,
        imageUrl: item.imageUrl,
        name: item.name,
        price: item.price,
        quantity: item.quantity
      })),
      totalPrice: this.totalPrice,
      dateTime: this.dateTime
    };
    console.log('Submitting order:', orderData);
    this.orderService.submitOrder(orderData).subscribe(response => {
      if (response.success) {
        // Перенаправление на страницу с подтверждением или успешным заказом
        this.cartService.clearCart();
        this.router.navigate(["/complete"]);
      } else {
        alert('Ошибка при оформлении заказа');
      }
    });
  }
  increaseQuantity(item: any): void {
    this.cartService.addToCart(item, 1);
    this.cart = this.cartService.getCart();
  }

  setDateLimits() {
    const today = new Date();
  
    const minDate = new Date(today);
    minDate.setDate(minDate.getDate() + 3); // Минимум через 3 дня
  
    const maxDate = new Date(today);
    maxDate.setDate(maxDate.getDate() + 25); // Максимум через 25 дней от сегодня
  
    this.minDate = minDate.toISOString().split('T')[0];
    this.maxDate = maxDate.toISOString().split('T')[0];
  
  }

  onDateChange(event: any) {
    const selectedDate = event.target.value; // Получаем строку "YYYY-MM-DD"
    const date = new Date(selectedDate);
    this.dateTime = date.toISOString(); // Преобразуем в ISO для отправки на back-end
  }

  decreaseQuantity(item: any): void {
    if (item.quantity > 1) {
      this.cartService.addToCart(item, -1); // Уменьшаем количество
    } 
    this.cart = this.cartService.getCart();
  }
  orderToCart(){
    this.router.navigate(['/cart']);
  }
  toDelivery(){
    this.isDelivery = true;
    this.isContact = false;
    this.isPayment = false;
  }
  deliveryToOrder(){
    this.isDelivery = false;
    this.isContact = true;
  }
  toPayment(){
    this.isDelivery = false;
    this.isPayment = true;
  }
  payment(){
    this.router.navigate(["/payment"]);
  }
}
