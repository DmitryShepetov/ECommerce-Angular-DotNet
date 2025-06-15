import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { OrderStatusHistoryService } from '../../core/services/orderStatusHistory.service';


@Component({
  selector: 'app-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './details.component.html',
  styleUrl: './details.component.scss'
})
export class DetailsComponent implements OnInit {
  orderId!: number;
  order: any;

  constructor(private route: ActivatedRoute, private orderService: OrderService, private orderStatusHistoryService: OrderStatusHistoryService) {}

  ngOnInit(): void {
    this.orderId = Number(this.route.snapshot.paramMap.get('id')); // Получаем id заказа из URL

    this.orderService.getOrderById(this.orderId).subscribe(res => {
      this.order = res;
      this.orderStatusHistoryService.getOrderStatusHistoryByIdStatus(this.orderId)
      .subscribe(statusHistory => {
        this.order.statusHistory = statusHistory || []; // Присваиваем массив статусов
      });
      
    });
  }
}
