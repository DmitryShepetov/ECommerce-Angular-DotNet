import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { OrderComponent } from '../order/order.component';

const validCard = {
  cardNumber: '5256 4342 4342 4342',
  expDate: '12/28',
  ccvNumber: '352',
  cardName: 'Dmitry Shepetov'
};

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss'
})


export class PaymentComponent {
  cardNumber: string = '5256 4256 4256 4256';
  expDate: string = '12/27';
  ccvNumber: string = '342';
  cardName: string = 'John Doe';
  isBackVisible: boolean = false;
  isVisa: any;



  constructor(private cartService: CartService, private router: Router) {}

  toggleBackCard() {
    this.isBackVisible = !this.isBackVisible;
  }

  showBackCard() {
    this.isBackVisible = true;
  }

  hideBackCard() {
    this.isBackVisible = false;
  }

  onCardNumberInput(event: Event) {
    const input = (event.target as HTMLInputElement).value.replace(/\D/g, '');
    this.cardNumber = this.formatCardNumber(input);

    if (input && input.charAt(0) === '5') {
      this.isVisa = false; // Показываем первую SVG (если начинается с 5)
    } else if (input && input.charAt(0) === '4') {
      this.isVisa = true; // Показываем вторую SVG (если начинается с 4)
    }
  }

  onCCVInput(event: Event) {
    this.ccvNumber = (event.target as HTMLInputElement).value.replace(/\D/g, '');
  }

  onExpDateInput(event: Event) {
    const input = (event.target as HTMLInputElement).value.replace(/\D/g, '');
    this.expDate = this.formatExpDate(input);
  }

  onCardNameInput(event: Event) {
    this.cardName = (event.target as HTMLInputElement).value;
  }

  private formatCardNumber(input: string): string {
    let formattedInput = '';
    for (let i = 0; i < input.length; i++) {
      if (i % 4 === 0 && i > 0) {
        formattedInput += ' ';
      }
      formattedInput += input[i];
    }
    return formattedInput;
  }

  private formatExpDate(input: string): string {
    let formattedInput = '';
    for (let i = 0; i < input.length; i++) {
      if (i % 2 === 0 && i > 0) {
        formattedInput += '/';
      }
      formattedInput += input[i];
    }
    return formattedInput;
  }

  pay() {
    if (
      this.cardNumber === validCard.cardNumber &&
      this.expDate === validCard.expDate &&
      this.ccvNumber === validCard.ccvNumber &&
      this.cardName === validCard.cardName
    ) {
      // Если все данные совпадают, оплата прошла успешно
      this.router.navigate(["/complete"]);
      this.cartService.clearCart();

    } else {
      // Если данные не совпадают, оплата не прошла
      this.router.navigate(["/not-paid"]);
    }
  }
}
