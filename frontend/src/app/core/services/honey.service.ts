import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HoneyService {
  private apiUrl = 'https://localhost:5001/api/Honey';
  constructor(private http: HttpClient) { }
  getHoney(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }
  getHoneyById(id: number) : Observable<any>{
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
  getHoneyWithCategory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/withCategory`);
  }
  updateHoney(honey: any, id: number): Observable<any>{
    return this.http.put<any>(`${this.apiUrl}/${id}`, honey);
  }
  uploadImage(file: FormData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/upload`, file);
  }
  addHoney(honey: any): Observable<any>{
    return this.http.post<any>(this.apiUrl, honey);
  }
  deleteHoney(id: number): Observable<any>{
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
