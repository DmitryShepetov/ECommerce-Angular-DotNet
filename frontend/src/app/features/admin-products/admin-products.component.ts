import { Component } from '@angular/core';
import { CategoryService } from '../../core/services/category.service';
import { HoneyService } from '../../core/services/honey.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [SidebarComponent, CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss'
})
export class AdminProductsComponent {
  honey: any[] = [];
  showAddForm = false;
  newProduct = this.getEmptyProduct();
  categories: any[] = [];
  sortNameActive = false;
  sortDirection: 'asc' | 'desc' = 'asc';


  constructor(private honeyService: HoneyService, 
      private categoryService: CategoryService, 
      ) {}
      ngOnInit(): void {
        this.honeyService.getHoneyWithCategory().subscribe((data) => {
          this.honey = data;
        });
        this.categoryService.getCategory().subscribe((data) => {
          this.categories = data;
        });
      }

      priceTypes: string[] = [
        "за 1 литр",
        "за 100 грамм",
        "за 1 килограмм",
        "за 100 миллилитров",
        "за 1 штуку",
        "за 10 штук",
        "за 100 штук"
      ];

      saveProduct(product: any, id: number) {
        
        this.newProduct = {...product};
        if (!this.validateForm()) {
          return;
        }
        const updatedProduct = {
          id: id,
          name: product.name,
          shortDesc: product.shortDesc,
          longDesc: product.longDesc,
          shelfLife: product.shelfLife,
          price: product.price,
          bju: product.bju,
          priceType: product.priceType,
          isFavorite: product.isFavorite,
          newHoney: product.newHoney,
          imageUrl: product.imageUrl,
          avaliable: product.avaliable,
          Categoryid: Number(product.categoryid)
        };
        this.honeyService.updateHoney(updatedProduct, id).subscribe(() => {
        }, error => {
          console.error('Ошибка при сохранении:', error);
        });
        product.isEditing= false;
      }

      toggleSort() {
        this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc'; // Меняем направление сортировки
        this.sortOrders();
        this.sortNameActive = true;
      }

      sortOrders() {
        this.honey.sort((a, b) => {
          if (this.sortDirection === 'asc') {
            return a.name.localeCompare(b.categoryName); // Сортировка по возрастанию
          } else {
            return b.name.localeCompare(a.categoryName); // Сортировка по убыванию
          }
        });
      }

      formErrors = {
        name: '',
        shortDesc: '',
        longDesc: '',
        shelfLife: '',
        price: '',
        bju: '',
        priceType: '',
        imageUrl: ''
      };

      validationMessages = {
        name: {
          required: 'Название обязательно',
          minlength: 'Название должно быть от 2 до 100 символов',
          maxlength: 'Название должно быть от 2 до 100 символов'
        },
        shortDesc: {
          required: 'Короткое описание обязательно',
          minlength: 'Короткое описание должно быть от 5 до 300 символов',
          maxlength: 'Короткое описание должно быть от 5 до 300 символов'
        },
        longDesc: {
          required: 'Полное описание обязательно',
          minlength: 'Полное описание должно быть минимум 10 символов'
        },
        shelfLife: {
          required: 'Срок хранения обязателен',
          maxlength: 'Срок хранения не более 50 символов'
        },
        price: {
          required: 'Цена обязательна',
          min: 'Цена должна быть больше 0'
        },
        bju: {
          required: 'БЖУ обязательно'
        },
        priceType: {
          required: 'Тип цены обязателен'
        },
        imageUrl: {
          required: 'Изображение обязательно'
        }
      };

      validateForm(): boolean {
        let isValid = true;
        const product = this.newProduct;

        // Проверка названия
        if (!product.name || product.name.trim().length < 2 || product.name.trim().length > 100) {
          this.formErrors.name = this.validationMessages.name.minlength;
          isValid = false;
        } else {
          this.formErrors.name = '';
        }

        // Проверка короткого описания
        if (!product.shortDesc || product.shortDesc.trim().length < 5 || product.shortDesc.trim().length > 300) {
          this.formErrors.shortDesc = this.validationMessages.shortDesc.minlength;
          isValid = false;
        } else {
          this.formErrors.shortDesc = '';
        }

        // Проверка длинного описания
        if (!product.longDesc || product.longDesc.trim().length < 10) {
          this.formErrors.longDesc = this.validationMessages.longDesc.minlength;
          isValid = false;
        } else {
          this.formErrors.longDesc = '';
        }

        // Проверка срока хранения
        if (!product.shelfLife || product.shelfLife.trim().length > 50) {
          this.formErrors.shelfLife = product.shelfLife ? this.validationMessages.shelfLife.maxlength : this.validationMessages.shelfLife.required;
          isValid = false;
        } else {
          this.formErrors.shelfLife = '';
        }

        // Проверка цены
        if (!product.price || product.price <= 0) {
          this.formErrors.price = this.validationMessages.price.min;
          isValid = false;
        } else {
          this.formErrors.price = '';
        }

        // Проверка БЖУ
        if (!product.bju || !product.bju.trim()) {
          this.formErrors.bju = this.validationMessages.bju.required;
          isValid = false;
        } else {
          this.formErrors.bju = '';
        }

        // Проверка типа цены
        if (!product.priceType) {
          this.formErrors.priceType = this.validationMessages.priceType.required;
          isValid = false;
        } else {
          this.formErrors.priceType = '';
        }

        // Проверка изображения
        if (!product.imageUrl) {
          this.formErrors.imageUrl = this.validationMessages.imageUrl.required;
          isValid = false;
        } else {
          this.formErrors.imageUrl = '';
        }

        return isValid;
      }

      editProduct(product: any) {
        this.honey.forEach(p => p.isEditing = false);
        product.isEditing = true;
        product.categoryid = product.categories?.id;
      }
    
      cancelEdit(product: any) {
        product.isEditing = false;
      }

      getEmptyProduct() {
        return {
          name: '',
          shortDesc: '',
          longDesc: '',
          shelfLife: '',
          price: 0,
          bju: '',
          priceType: '',
          isFavorite: false,
          newHoney: false,
          avaliable: true,
          imageUrl: '',
          Categoryid: 0
        };
      }

      toggleAddProduct() {
        this.showAddForm = !this.showAddForm;
        if (!this.showAddForm) {
          this.newProduct = this.getEmptyProduct();
        }
      }

      deleteProduct(product: any, id: number){
        this.honeyService.deleteHoney(id).subscribe(response => {
        }, error => {
          console.error('Ошибка при сохранении:', error);
        });
      }

      onFileSelected(event: Event) {
        const file = (event.target as HTMLInputElement).files?.[0];
        if (!file) return;
      
        const formData = new FormData();
        formData.append('file', file);
      
        this.honeyService.uploadImage(formData).subscribe(response => {
          console.log(response.url);
          this.newProduct.imageUrl = response.url;
          console.log(this.newProduct.imageUrl);
        }, error => {
          console.error('Ошибка загрузки файла:', error);
        });
      }

      addProduct() {
        if (!this.validateForm()) {
          return;
        }
        this.honeyService.addHoney(this.newProduct).subscribe(response => {
        }, error => {
          console.log(this.newProduct);
          console.error('Ошибка при сохранении:', error);
        });
      }
}
