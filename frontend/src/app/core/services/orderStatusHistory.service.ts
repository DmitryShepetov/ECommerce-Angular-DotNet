import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderStatusHistoryService {
  private apiUrl = 'https://localhost:5001/api/OrderStatusHistory'; // Ваш API для отправки заказов

  constructor(private http: HttpClient) {}

  getOrderStatusHistoryByIdStatus(orderId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${orderId}`);
  }
  addOrderStatusHistory(orderStatusHistory: any): Observable<any>{
    return this.http.post<any>(`${this.apiUrl}`, orderStatusHistory);
  }
}
