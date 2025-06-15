import { Component, OnInit } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit {
    cart: any[] = [];
  
    constructor(private cartService: CartService, private router: Router) {}
  
    ngOnInit(): void {
      this.cart = this.cartService.getCart();
    }
  
    removeFromCart(productId: number): void {
      this.cartService.removeFromCart(productId);
      this.cart = this.cartService.getCart();
    }
  
    clearCart(): void {
      this.cartService.clearCart();
      this.cart = [];
    }
  
    increaseQuantity(item: any): void {
      this.cartService.addToCart(item, 1);
      this.cart = this.cartService.getCart();
    }
  
    decreaseQuantity(item: any): void {
      if (item.quantity > 1) {
        this.cartService.addToCart(item, -1); // Уменьшаем количество
      } 
      this.cart = this.cartService.getCart();
    }
  
    getTotalPrice(): number {
      return this.cart.reduce((total, item) => total + item.price * item.quantity, 0);
    }
  
    checkout(): void {
      this.router.navigate(['/order']);
    }
  }
  