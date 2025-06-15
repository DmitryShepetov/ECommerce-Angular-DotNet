import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../core/services/order.service';
import { OrderStatusHistoryService } from '../../core/services/orderStatusHistory.service';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [SidebarComponent, CommonModule, FormsModule],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.scss'
})
export class AdminOrdersComponent implements OnInit {
  orders: any[] = [];
  isEdit: boolean = false;
  selectedStatus: string = "";
  statuses: string[] = ["Выполнен", "Отменён", "Принят в обработку", "Передан в доставку"];
  sortDirection: 'asc' | 'desc' = 'asc';
  statusSortDirection: 'asc' | 'desc' = 'asc';
  statusesOrder = ['Принят в обработку', 'Передан в доставку', 'Выполнен', 'Отменён'];
  sortIdActive = true;
  sortStatusActive = false;

  constructor(private orderService: OrderService, private orderStatusHistoryService: OrderStatusHistoryService){}

  ngOnInit(): void {
    this.orderService.getAllOrders().subscribe(data => {
      this.orders = data
      const statusRequests = this.orders.map(order => 
        this.orderStatusHistoryService.getOrderStatusHistoryByIdStatus(order.id).toPromise()
      );
      Promise.all(statusRequests).then(statusHistories => {
        this.orders.forEach((order, index) => {
          order.statusHistory = statusHistories[index] || []; // Присваиваем массив статусов
        });
        this.sortOrders();
        this.sortOrdersByStatus();
      });
    });
  }

  toggleSort() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc'; // Меняем направление сортировки
    this.sortOrders();
    this.sortStatusActive = false;
    this.sortIdActive = true;
  }

  sortOrders() {
    this.orders.sort((a, b) => {
      if (this.sortDirection === 'asc') {
        return a.id - b.id; // Сортировка по возрастанию
      } else {
        return b.id - a.id; // Сортировка по убыванию
      }
    });
  }

  toggleStatusSort() {
    this.statusSortDirection = this.statusSortDirection === 'asc' ? 'desc' : 'asc';
    this.sortOrdersByStatus();
    this.sortIdActive = false;
    this.sortStatusActive = true;
  }

  // Сортировка заказов по статусу
  sortOrdersByStatus() {
    this.orders.sort((a, b) => {
      const statusAIndex = this.statusesOrder.indexOf(a.statusHistory[a.statusHistory.length - 1]?.status || '');
      const statusBIndex = this.statusesOrder.indexOf(b.statusHistory[b.statusHistory.length - 1]?.status || '');
      
      if (this.statusSortDirection === 'asc') {
        return statusAIndex - statusBIndex; // Сортировка по возрастанию
      } else {
        return statusBIndex - statusAIndex; // Сортировка по убыванию
      }
    });
  }

  edit(order: any) {
    order.isEdit = true;
    order.selectedStatus = order.statusHistory[order.statusHistory.length - 1]?.status || "";
}

  cancel(order: any) {
      order.isEdit = false;
  }

  saveStatus(order: any) {
    if (!order.selectedStatus) return;
  
    const updatedStatus = {
      id: 0, // Сервер, скорее всего, сам сгенерирует ID
      status: order.selectedStatus,
      changedAt: new Date().toISOString(),
      orderId: order.id
    };
  
    this.orderStatusHistoryService.addOrderStatusHistory(updatedStatus).subscribe(() => {
      order.statusHistory.push(updatedStatus);
      order.isEdit = false;
    }, error => {
      console.error('Ошибка при обновлении статуса:', error);
    });
  }

  getStatusClass(status: string | undefined): string {
    switch (status) {
      case 'Выполнен':
        return 'text-green-500 bg-green-100'; // Зеленый текст и фон
      case 'Отменён':
        return 'text-red-500 bg-red-100'; // Красный текст и фон
      case 'Передан в доставку':
        return 'text-purple-500 bg-purple-100';
      case 'Принят в обработку':
        return 'text-orange-500 bg-orange-100'; // Оранжевый текст и фон
      default:
        return 'text-blue-600 bg-blue-100'; // Серый текст и фон
    }
  }

  
}
