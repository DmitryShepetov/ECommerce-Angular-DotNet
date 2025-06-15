import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { UserStoreService } from '../../../core/services/user-store.service';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  public lastName: string = "";
  public firstName: string = "";
  public image: string = "";
  public role: string | null = "";

    constructor(
      private auth: AuthService, 
      private userStore: UserStoreService,
      private router: Router){
      
    }
    
    ngOnInit() {

      this.auth.getCurrentUser().subscribe(val => {
        this.firstName = val.firstName;
        this.lastName = val.lastName;
        this.image = val.image;
      })

      this.role = this.auth.getRole();
    }

    isActive(route: string): boolean {
      return this.router.url === route;
    }
    handleImageError(event: Event) {
      const imgElement = event.target as HTMLImageElement;
      imgElement.src = 'assets/profileAvatar.png'; // путь к изображению-заглушке
    }
}
