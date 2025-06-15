import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { NgToastService } from 'ng-angular-popup';
import { Router } from '@angular/router';

export const tokenInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const authService = inject(AuthService);
  const toast = inject(NgToastService);
  const router = inject(Router);

  // Skip for auth requests except those that need auth
  const authRoutes = ['login', 'register', 'google-auth'];
  const isAuthRequest = authRoutes.some(route => req.url.includes(route));
  
  
  if (isAuthRequest && !req.url.includes('profile')) {
    return next(req);
  }

  const token = authService.getToken();
  const authReq = token 
    ? req.clone({ 
        setHeaders: { Authorization: `Bearer ${token}` },
        withCredentials: true
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('refresh-token')) {
        return handleUnauthorizedError(authService, req, next, router, toast);
      }
      return throwError(() => error);
    })
  );
};

function handleUnauthorizedError(
  authService: AuthService,
  req: HttpRequest<any>,
  next: HttpHandlerFn,
  router: Router,
  toast: NgToastService
) {
  // Если у нас нет токена - сразу редирект на логин
  if (!authService.getToken()) {
    return throwError(() => new Error('No token available'));
  }

  return authService.refreshToken().pipe(
    switchMap(() => {
      const newToken = authService.getToken();
      if (!newToken) {
        throw new Error('Refresh failed');
      }
      
      const newReq = req.clone({
        setHeaders: { Authorization: `Bearer ${newToken}` },
        withCredentials: true
      });
      return next(newReq);
    }),
    catchError(refreshError => {
      authService.logOutAll().subscribe();
      toast.warning('Сессия истекла', 'Пожалуйста, войдите снова', 5000);
      router.navigate(['/login']);
      return throwError(() => refreshError);
    })
  );
}
