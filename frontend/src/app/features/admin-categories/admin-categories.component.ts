import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../core/services/category.service';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [SidebarComponent, CommonModule, FormsModule],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.scss'
})
export class AdminCategoriesComponent {
    showAddForm = false;
    newCategory = this.getEmptyCategory();
    categories: any[] = [];
    sortNameActive = false;
    sortDirection: 'asc' | 'desc' = 'asc';

    constructor(private categoryService: CategoryService) {}
        ngOnInit(): void {
          this.categoryService.getCategory().subscribe((data) => {
            this.categories = data;
          });
        }
  
        saveCategory(category: any, id: number) {
          const updatedCategory = {
            id: id,
            categoryName: category.categoryName,
            desc: category.desc,
          };
          this.categoryService.updateCategory(updatedCategory, id).subscribe(() => {
          }, error => {
            console.error('Ошибка при сохранении:', error);
          });
          category.isEditing= false;
          console.log(updatedCategory);
        }
        
        toggleSort() {
          this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc'; // Меняем направление сортировки
          this.sortOrders();
          this.sortNameActive = true;
        }
      
        sortOrders() {
          this.categories.sort((a, b) => {
            if (this.sortDirection === 'asc') {
              return a.categoryName.localeCompare(b.categoryName); // Сортировка по возрастанию
            } else {
              return b.categoryName.localeCompare(a.categoryName); // Сортировка по убыванию
            }
          });
        }

        editCategory(category: any) {
          this.categories.forEach(p => p.isEditing = false);
          category.isEditing = true;
        }
      
        cancelEdit(category: any) {
          category.isEditing = false;
        }
  
        getEmptyCategory() {
          return {
            categoryName: '',
            desc: ''
          };
        }
  
        toggleAddCategory() {
          this.showAddForm = !this.showAddForm;
          if (!this.showAddForm) {
            this.newCategory = this.getEmptyCategory();
          }
        }
  
        deleteCategory(category: any, id: number){
          this.categoryService.deleteCategory(id).subscribe((response: any) => {
            category.isEditing = false;
            this.categories = this.categories.filter(category => category.id !== id);
          }, error => {
            console.error('Ошибка при сохранении:', error);
          });
        }
  
        addCategory() {
          // Проверка валидности формы перед отправкой
          if (this.newCategory.categoryName.length < 2 || this.newCategory.categoryName.length > 50) {
            console.error('Название категории должно быть от 2 до 50 символов');
            return;
          }
          
          if (!this.newCategory.desc || this.newCategory.desc.length > 500) {
            console.error('Описание не должно быть пустым и содержать не более 500 символов');
            return;
          }

          this.categoryService.addCategory(this.newCategory).subscribe((response: any) => {
            this.categories.push(response); // Лучше использовать объект из ответа сервера
            this.newCategory = this.getEmptyCategory();
            this.showAddForm = false;
          }, error => {
            console.error('Ошибка при сохранении:', error);
          });
        }
}
