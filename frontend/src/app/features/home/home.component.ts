import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA, HostListener  } from '@angular/core';
import { HoneyService } from '../../core/services/honey.service';
import { CategoryService } from '../../core/services/category.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { FormsModule } from '@angular/forms';
import { ModalService } from '../../core/services/modal.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class HomeComponent implements OnInit {
  honey: any[] = [];
  category: any[] = [];
  products: any;
  selectedCategoryId: number = 1;
  filteredHoneyBySlider: any[] = [];
  filteredHoney: any[] = [];
  selectedProduct: any | null = null;
  isModalOpen = false;
  quantity: number = 1;
  price: number = 0;


  constructor(private honeyService: HoneyService, 
    private categoryService: CategoryService, 
    private cartService: CartService,
    private modalService: ModalService) {}

  ngOnInit(): void {
    this.honeyService.getHoney().subscribe((data) => {
      this.honey = data;
      this.filteredHoneyBySlider = this.honey.filter(item => item.isFavorite);
      console.log(this.filteredHoneyBySlider);
    });
    
    this.categoryService.getCategory().subscribe((data) => {
      this.category = data;
    });
  }

  openProductModal(productId: number, event: MouseEvent): void {
    event.stopPropagation();
    this.modalService.openModal(productId);
  }

  get filteredProducts() {
    return this.honey.filter(product => product.categoryid === this.selectedCategoryId);
  }
  selectCategory(categoryId: number): void {
    this.selectedCategoryId = categoryId; // Устанавливаем выбранную категорию
  }


  closeProductModal() {
    this.selectedProduct = null;
    this.isModalOpen = false;
    const modal = document.getElementById('extralarge-modal');
    if (modal) {
      modal.classList.add('hidden');
    }
  }

  confirmToSend(): void {
    if (this.selectedProduct) {
      this.cartService.addToCart(this.selectedProduct, this.quantity);
      console.log('Товар добавлен в корзину:', this.selectedProduct, 'Количество:', this.quantity);
    }
  }

    @HostListener('document:click', ['$event'])
    onClickOutside(event: MouseEvent) {
      const target = event.target as HTMLElement;
      if (!target.closest('.relative.bg-white')) {
        this.closeProductModal();
      }
    }
  increaseQuantity() {
    this.quantity++;
    this.price += this.selectedProduct.price;
  }

  decreaseQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
      this.price -= this.selectedProduct.price;
    }
  }
  
  updatePrice() {
    this.price = this.quantity * this.selectedProduct.price;
  }
}

