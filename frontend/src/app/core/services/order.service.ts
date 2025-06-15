import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = 'https://localhost:5001/api/Order';

  constructor(private http: HttpClient) {}

  submitOrder(order: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, order); // Отправляем заказ на сервер
  }
  getOrderByPhoneNumber(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/by-phone`);
  }
  getAllOrders(): Observable<any[]>{
    return this.http.get<any[]>(this.apiUrl);
  }
  getOrderById(orderId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${orderId}`);
  }
}
