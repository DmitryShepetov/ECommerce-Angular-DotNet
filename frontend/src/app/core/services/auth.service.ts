import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthResponseDto, TokenRefreshResponseDto, UserProfileDto } from '../interfaces/auth.interface';
import { NgToastService } from 'ng-angular-popup';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `https://localhost:5001/api/auth/`;
  private tokenSubject = new BehaviorSubject<string | null>(null);
  private userPayload: any;
  public token$ = this.tokenSubject.asObservable();
  

  constructor(
    private http: HttpClient,
    
    private router: Router,
    private jwtHelper: JwtHelperService,
    private toast: NgToastService
  ) {
    this.userPayload = this.decodeToken();
  }

  // Auth methods
  login(loginObj: { username: string; password: string }): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}login`, loginObj, { withCredentials: true }).pipe(
      tap(response => {
        this.storeToken(response.accessToken);
        this.userPayload = this.decodeToken();
      })
    );
  }

  register(userObj: any): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}register`, userObj, { withCredentials: true }).pipe(
      tap(response => {
        this.storeToken(response.accessToken);
        this.userPayload = this.decodeToken();
      })
    );
  }

  refreshToken(): Observable<TokenRefreshResponseDto> {
    return this.http.post<TokenRefreshResponseDto>(`${this.apiUrl}refresh-token`, {}, { withCredentials: true }).pipe(
      tap(response => {
        this.storeToken(response.accessToken);
      }),
      catchError(error => {
        this.clearToken();
        return throwError(() => error);
      })
    );
  }


  logOutAll(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}revoke-token`, {}, { withCredentials: true }).pipe(
      tap(() => {
        this.clearToken();
        this.router.navigate(['']);
      })
    );
  }

  logOut(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}log-out`, {}, { withCredentials: true }).pipe(
      tap(() => {
        this.clearToken();
        this.router.navigate(['']);
      })
    );
  }

  // Token management
  public storeToken(token: string): void {
    this.tokenSubject.next(token);
    this.userPayload = this.decodeToken();
  }

  private clearToken(): void {
    this.tokenSubject.next(null);
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  // User info
  private decodeToken(): any {
    const token = this.getToken();
    return token ? this.jwtHelper.decodeToken(token) : null;
  }

  getCurrentUser(): Observable<UserProfileDto> {
    return this.http.get<UserProfileDto>(`${this.apiUrl}profile`);
  }

  uploadImage(file: File): Observable<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ imageUrl: string }>(`${this.apiUrl}upload-image`, formData);
  }
  getRole(): string | null {
    if (this.userPayload && this.userPayload.role) {
      return this.userPayload.role;
    }
    return null;
  }
}
