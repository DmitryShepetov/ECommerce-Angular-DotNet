import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cart: any[] = [];
  private cartSubject = new BehaviorSubject<any[]>(this.cart);

  constructor() {
    if (typeof window !== 'undefined' && window.localStorage) {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cart = JSON.parse(savedCart);
    }
}
    if (typeof window !== 'undefined' && window.localStorage) {
        this.loadCart();
      }
  }
  
  private loadCart(): void {
    if (typeof window !== 'undefined' && window.localStorage) {
    const storedCart = localStorage.getItem('cart');
    this.cart = storedCart ? JSON.parse(storedCart) : [];
    this.cartSubject.next(this.cart);
    }
  }

  getCart(): any[] {
    return this.cart;
  }

  addToCart(product: any, quantity: number): void {
    const existingProduct = this.cart.find(item => item.id === product.id);
    if (existingProduct) {
      existingProduct.quantity += quantity;
    } else {
      this.cart.push({ ...product, quantity });
    }
    this.saveCart();
  }

  getCartObservable() {
    return this.cartSubject.asObservable(); // ✅ Позволяет подписываться на изменения корзины
  }

  removeFromCart(productId: number): void {
    this.cart = this.cart.filter(item => item.id !== productId);
    this.saveCart();
  }

  clearCart(): void {
    this.cart = [];
    this.saveCart();
  }

  private saveCart(): void {
    if (typeof window !== 'undefined' && window.localStorage) {
        localStorage.setItem('cart', JSON.stringify(this.cart));
        this.cartSubject.next(this.cart);
    }
  }
}
