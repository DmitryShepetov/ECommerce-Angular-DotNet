import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserStoreService } from '../../../core/services/user-store.service';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';
import { HoneyService } from '../../../core/services/honey.service';
import { ModalService } from '../../../core/services/modal.service';


@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  public firstName: string = "";
  public lastName: string = "";
  public email: string = "";
  public image: string = "";
  public role: string | null = "";
  isDropdownOpen = false;
  public isLogginIn: boolean = false;
  cartItemCount: number = 0;
  searchQuery: string = "";  // Строка для поиска
  products: any[] = [];  // Массив товаров, которые будут отображаться
  filteredProducts: any[] = [];  // Отфильтрованные товары


  constructor(
    private router: Router, 
    private auth: AuthService, 
    private cartService: CartService,
    private userStore: UserStoreService,
    private honeyService: HoneyService,
    private modalService: ModalService) {}
  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  ngOnInit() {
    this.isLogginIn = this.auth.isLoggedIn();
    this.auth.getCurrentUser().subscribe(val => {
      this.firstName = val.firstName;
      this.lastName = val.lastName;
      this.image = val.image;
      this.email = val.email;
    })
    this.role = this.auth.getRole();

    this.honeyService.getHoney().subscribe(product => {
      this.products = product;
      this.filterProducts();
    });

    this.cartService.getCartObservable().subscribe(cart => {
      this.cartItemCount = cart.length;
    });

    // ✅ Загружаем текущее состояние корзины
    this.cartItemCount = this.cartService.getCart().length;

  }
  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.dropdown')) {
      this.isDropdownOpen = false;
    }
  }
  onLogin() {
    this.router.navigate(['/login']);
  }

  onSearch() {
    this.filterProducts();
  }

  openProductModal(productId: number, event: MouseEvent): void {
    event.stopPropagation();
    this.modalService.openModal(productId);
  }

  filterProducts() {
    if (this.searchQuery.trim() === '') {
      this.filteredProducts = [];  // Если нет запроса, показываем пустой список
    } else {
      this.filteredProducts = this.products.filter(product => 
        product.name.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    }
  }

  handleImageError(event: Event) {
  const imgElement = event.target as HTMLImageElement;
  imgElement.src = 'assets/profileAvatar.png'; // путь к изображению-заглушке
}

  onRegister() {
    this.router.navigate(['/signup']);
  }
  onProfile() {
    this.router.navigate(['/profile']);
  }
  logOut(){
    this.auth.logOut().subscribe({
    next: () => {
        this.isLogginIn = false;
        this.role = "";
        this.router.navigate(['']);
      },
      error: err => {
        console.error("Ошибка при выходе:", err);
      }
    });;
  }

  logOutAll(){
    this.auth.logOutAll().subscribe({
    next: () => {
        this.isLogginIn = false;
        this.role = "";
        this.router.navigate(['']);
      },
      error: err => {
        console.error("Ошибка при выходе:", err);
      }
    });
  }

}
