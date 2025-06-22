import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class UserService {
  private apiUrl = "https://localhost:5001/api/auth/users";
  constructor(private http: HttpClient, private router: Router) {}

  updateUserProfile(updatedUserData: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update-profile`, updatedUserData);
  }

  uploadImage(file: FormData): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/upload-image`, file);
  }

  getUsers(): Observable<any>{
    return this.http.get<any>(this.apiUrl);
  }
}
