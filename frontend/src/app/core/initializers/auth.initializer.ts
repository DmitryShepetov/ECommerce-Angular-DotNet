import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, of } from 'rxjs';


export function authInitializer() {
  const authService = inject(AuthService);
  return () =>
    authService.refreshToken()
      .pipe(catchError(() => of(null))) // Без редиректа и logout
      .toPromise();
}