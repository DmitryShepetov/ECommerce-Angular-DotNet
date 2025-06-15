import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { NgToastModule, ToasterPosition } from 'ng-angular-popup';
import { CookieBannerComponent } from './shared/components/cookie-banner/cookie-banner.component';
import { ProductModalComponent } from './shared/components/product-modal/product-modal.component';



@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HeaderComponent, FooterComponent, NgToastModule, CookieBannerComponent, ProductModalComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  toasterPosition = ToasterPosition;
  title = 'Honey House';
}
