import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { HoneyService } from '../../../core/services/honey.service';
import { CartService } from '../../../core/services/cart.service';
import { ModalService } from '../../../core/services/modal.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


@Component({
  selector: 'app-product-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})
export class ProductModalComponent implements OnInit, OnDestroy {
  isModalOpen = false;
  selectedProduct: any = null;
  quantity: number = 1;
  priceType: string = '';
  price: number = 0;
  
  private modalSubscription!: Subscription;

  constructor(
    private honeyService: HoneyService,
    private cartService: CartService,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    this.modalSubscription = this.modalService.modalState$.subscribe(productId => {
      if (productId) {
        this.honeyService.getHoneyById(productId).subscribe(product => {
          this.selectedProduct = product;
          this.price = product.price;
          this.quantity = 1;
          this.priceType = product.priceType
          this.isModalOpen = true;
        });
      } else {
        this.closeModal();
      }
    });
  }

  onBackdropClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;

    // Закрыть только если клик был именно по фону, а не по содержимому модалки
    if (target.id === 'extralarge-modal') {
      this.closeModal();
    }
  }

  ngOnDestroy(): void {
    if (this.modalSubscription) {
      this.modalSubscription.unsubscribe();
    }
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedProduct = null;
    this.modalService.closeModal();
  }

  increaseQuantity(): void {
    this.quantity++;
    this.updatePrice();
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
      this.updatePrice();
    }
  }

  updatePrice(): void {
    this.price = this.quantity * (this.selectedProduct?.price || 0);
  }

  addToCart(): void {
    if (this.selectedProduct) {
      this.cartService.addToCart(this.selectedProduct, this.quantity);
      this.closeModal();
    }
  }

  @HostListener('document:keydown.escape', ['$event'])
  onKeydownHandler(event: KeyboardEvent) {
    this.closeModal();
  }
}