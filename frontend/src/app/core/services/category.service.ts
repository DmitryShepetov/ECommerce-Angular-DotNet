import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = 'https://localhost:5001/api/Category';
  constructor(private http: HttpClient) { }
  getCategory(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
  getCategoryById(categoryId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${categoryId}`)
  }
  updateCategory(category: any, id: number): Observable<any>{
    return this.http.put<any>(`${this.apiUrl}/${id}`, category);
  }
  addCategory(category: any): Observable<any>{
    return this.http.post<any>(this.apiUrl, category);
  }
  deleteCategory(id: number): Observable<any>{
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}