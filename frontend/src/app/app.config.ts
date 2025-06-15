import { APP_INITIALIZER, ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app-routing.module';
import { provideClientHydration } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule, provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { tokenInterceptor } from './core/interceptors/token.interceptor';
import { provideServerRendering } from '@angular/platform-server';
import { JWT_OPTIONS, JwtHelperService } from '@auth0/angular-jwt';
import { provideAnimations } from '@angular/platform-browser/animations';
import { authInitializer } from './core/initializers/auth.initializer';
import { AuthService } from './core/services/auth.service';

// export const appConfig: ApplicationConfig = {
//   providers: [provideRouter(routes), 
// provideClientHydration(), provideHttpClient(withFetch(),withInterceptors([tokenInterceptor])), ]
// };
export const appConfig: ApplicationConfig = {
  providers: [
    provideAnimations(),
    provideRouter(routes), 
    provideClientHydration(), 
    provideHttpClient(withFetch(),withInterceptors([tokenInterceptor])),
  // Провайдер для JwtHelperService
    JwtHelperService,
    {
      provide: JWT_OPTIONS,
      useValue: {}
    },
    {
      provide: APP_INITIALIZER,
      useFactory: authInitializer,
      deps: [AuthService],
      multi: true
    } ]
};
