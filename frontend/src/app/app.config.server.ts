import { mergeApplicationConfig, ApplicationConfig, inject } from '@angular/core';
import { provideServerRendering } from '@angular/platform-server';
import { appConfig } from './app.config';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { register } from 'swiper/element';

const serverConfig: ApplicationConfig = {
  providers: [
    provideServerRendering()
  ]
};

export const config = mergeApplicationConfig(appConfig, serverConfig);
