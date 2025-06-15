import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { NgToastService } from 'ng-angular-popup';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private auth: AuthService, 
    private router: Router, 
    private toast: NgToastService
  ) {}

  canActivate(): boolean {
    if (this.auth.isLoggedIn() && this.auth.getRole() === 'Admin') {
      return true; // Разрешаем доступ
    } else {
      this.toast.danger('У вас нет доступа к этой странице', 'Ошибка', 5000);
      this.router.navigate([''])
      return false;
    }
  }
}
