import { Component, OnInit } from '@angular/core';
import { CookieService } from '../../../core/services/cookie.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cookie-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cookie-banner.component.html',
  styleUrl: './cookie-banner.component.scss'
})
export class CookieBannerComponent implements OnInit {
  showBanner = true;

  constructor(private cookieService: CookieService) {}

  ngOnInit() {
    if (typeof document !== 'undefined') {
    if (this.cookieService.getCookie('cookiesAccepted')) {
      this.showBanner = false;
    }
  }
  }

  acceptCookies() {
    this.cookieService.setCookie('cookiesAccepted', 'true', 365);
    this.showBanner = false;
  }
}