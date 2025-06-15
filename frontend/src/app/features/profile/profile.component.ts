import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { OrderService } from '../../core/services/order.service';
import { UserStoreService } from '../../core/services/user-store.service';
import { AuthService } from '../../core/services/auth.service';
import { OrderStatusHistoryService } from '../../core/services/orderStatusHistory.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class AdminComponent implements OnInit {

  orders: any[] = [];
  phone: string = "";
  email: string = "";
  firstName: string = "";
  lastName: string = "";
  image: string = "";
  username: string = "";
  date: string = "";


  isChanged = false;

  constructor(private router: Router, 
    private orderService: OrderService, 
    private userService: UserService,
    private auth: AuthService, 
    private orderStatusHistoryService: OrderStatusHistoryService) {}


    onUpdateProfile(): void {
      const updatedUserData = {
        username: this.username,
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        phone: this.phone,
        image: this.image,
        date: this.date
      };
    
      this.userService.updateUserProfile(updatedUserData).subscribe(response => {
        this.isChanged = false;
        console.log("Всё прошло успешно")
        console.log(updatedUserData);
      }, error => {
        console.log("Что-то пошло не так")
        console.log(updatedUserData);
      });
    }

    ngOnInit(): void {

      this.auth.getCurrentUser().subscribe(val => {
        this.firstName = val.firstName,
        this.username = val.username,
        this.lastName = val.lastName,
        this.phone = val.phone,
        this.email = val.email,
        this.date = val.date,
        this.image = val.image
      });
      this.orderService.getOrderByPhoneNumber().subscribe(res => {
        this.orders = res;

        const statusRequests = this.orders.map(order => 
          this.orderStatusHistoryService.getOrderStatusHistoryByIdStatus(order.id).toPromise()
        );

        Promise.all(statusRequests).then(statusHistories => {
          this.orders.forEach((order, index) => {
            order.statusHistory = statusHistories[index] || []; // Присваиваем массив статусов
          });
        });
      });
    }

    toggleUpdateProfile() {
      this.isChanged = !this.isChanged;
    }

  detailed(orderId: number): void {
    this.router.navigate(['/details', orderId]);
  }

  onDateChange(event: any) {
    const selectedDate = event.target.value; // Получаем строку "YYYY-MM-DD"
    const date = new Date(selectedDate);
    this.date = date.toISOString(); // Преобразуем в ISO для отправки на back-end
    console.log(this.date);
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
  
    const formData = new FormData();
    formData.append('file', file);
  
    this.userService.uploadImage(formData).subscribe(response => {
      console.log(response.url);
      this.image = response.url;
    }, error => {
      console.error('Ошибка загрузки файла:', error);
    });
  }

  getStatusClass(status: string | undefined): string {
    switch (status) {
      case 'Выполнен':
        return 'text-green-500 bg-green-100'; // Зеленый текст и фон
      case 'Отмена':
        return 'text-red-500 bg-red-100'; // Красный текст и фон
      case 'Передан в обработку':
        return 'text-orange-500 bg-orange-100'; // Оранжевый текст и фон
      default:
        return 'text-blue-600 bg-blue-100'; // Серый текст и фон
    }
  }

    handleImageError(event: Event) {
      const imgElement = event.target as HTMLImageElement;
      imgElement.src = 'assets/profileAvatar.png'; // путь к изображению-заглушке
    }
}
