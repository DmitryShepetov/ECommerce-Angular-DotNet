import { Component, OnInit } from '@angular/core';
import { NgToastModule, NgToastService, ToasterPosition } from 'ng-angular-popup';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { CommonModule } from '@angular/common';
import { UserStoreService } from '../../core/services/user-store.service';
import { OrderService } from '../../core/services/order.service';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';
import { NgChartsModule } from 'ng2-charts';
import { OrderStatusHistoryService } from '../../core/services/orderStatusHistory.service';
import { ChartConfiguration } from 'chart.js';
import { HoneyService } from '../../core/services/honey.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgToastModule, CommonModule, SidebarComponent, NgChartsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  public users: any = [];
  public ordersCategory: any = [];
  public honeys: any = [];

  public productCategoryChartData: ChartConfiguration<'pie'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'],
      },
    ],
  };

  productCategoryChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom',
      },
    },
  };

  orderChartData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Сумма заказа',
        borderColor: '#3b82f6',
        backgroundColor: 'rgba(59, 130, 246, 0.2)',
        tension: 0.4,
        fill: false,
        pointRadius: 5,
      },
      {
        data: [],
        label: 'Средняя сумма',
        borderColor: '#f87171',
        borderDash: [10, 5],
        pointRadius: 0,
        fill: false,
        tension: 0.1,
      }
    ]
  };

  orderChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };

  // Pie Chart Data
  orderStatusChartData: ChartConfiguration<'pie'>['data'] = {
    labels: ['Выполнен', 'Отменён'],
    datasets: [
      {
        data: [0, 0],
        backgroundColor: ['#4ade80', '#f87171'],
        hoverBackgroundColor: ['#22c55e', '#ef4444']
      }
    ]
  };

  orderStatusChartOptions: ChartConfiguration<'pie'>['options'] = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  constructor(
    private toast: NgToastService, 
    private order: OrderService,
    private orderStatusHistoryService: OrderStatusHistoryService, 
    private user: UserService,
    private honey: HoneyService,
  ) {}

  ngOnInit() {
    this.user.getUsers().subscribe((res: any[]) => {
      this.users = res;
    });

    this.order.getAllOrders().subscribe((res: any[]) => {
      this.ordersCategory = res;

      const prices = this.ordersCategory.map((o: any) => o.totalPrice || 0);
      const labels = this.ordersCategory.map((o: any) =>
        new Date(o.dateTime).toLocaleDateString('ru-RU', {
          day: 'numeric',
          month: 'long',
          year: 'numeric'
        })
      );

      const avgPrice = prices.reduce((sum: number, price: number) => sum + price, 0) / prices.length;

      this.orderChartData.labels = labels;
      this.orderChartData.datasets[0].data = prices;
      this.orderChartData.datasets[1].data = Array(prices.length).fill(avgPrice);

      const statusRequests = this.ordersCategory.map((order: any) =>
        this.orderStatusHistoryService.getOrderStatusHistoryByIdStatus(order.id).toPromise()
      );

      Promise.all(statusRequests).then((statusHistories: any[]) => {
          let completedCount = 0;
          let cancelledCount = 0;

          this.ordersCategory.forEach((order: any, index: number) => {
                const history = statusHistories[index] || [];
                order.statusHistory = history;

                if (history.length > 0) {
                  const latestStatus = history[history.length - 1].status; // предположим, это строка: "Выполнен", "Отменён"
                  if (latestStatus === 'Выполнен') {
                    completedCount++;
                  } else if (latestStatus === 'Отменён') {
                    cancelledCount++;
                  }
                }
              });
            this.orderStatusChartData.datasets[0].data = [completedCount, cancelledCount];
      });
    });

    // Получаем товары с категориями
    this.honey.getHoneyWithCategory().subscribe((res: any[]) => {
      this.honeys = res;
      console.log(this.honeys);

      // Подсчитываем количество товаров по категориям
      const categoryCounts: { [key: string]: number } = {};

      // Проходим по всем товарам и считаем их в категории
      this.honeys.forEach((product: any) => {
        const categoryName = product.categories.categoryName;
        if (categoryCounts[categoryName]) {
          categoryCounts[categoryName]++;
        } else {
          categoryCounts[categoryName] = 1;
        }
      });

      // Обновляем данные для графика
      const categories = Object.keys(categoryCounts);
      const counts = Object.values(categoryCounts);

      this.productCategoryChartData.labels = categories;
      this.productCategoryChartData.datasets[0].data = counts;
    });
  }
}
