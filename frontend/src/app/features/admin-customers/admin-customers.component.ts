import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { NgToastModule, NgToastService, ToasterPosition } from 'ng-angular-popup';
import { UserStoreService } from '../../core/services/user-store.service';
import { SidebarComponent } from '../../shared/components/sidebar/sidebar.component';

@Component({
  selector: 'app-admin-customers',
  standalone: true,
  imports: [NgToastModule, SidebarComponent, CommonModule],
  templateUrl: './admin-customers.component.html',
  styleUrl: './admin-customers.component.scss'
})
export class AdminCustomersComponent implements OnInit {
    public users: any = [];
    public role: string | null = "";
    public userName: string = "";
    toasterPosition = ToasterPosition;
    constructor(
      private toast: NgToastService, 
      private auth: AuthService, 
      private user: UserService, 
      private userStore: UserStoreService){
      
    }
    
    ngOnInit() {
      this.user.getUsers().subscribe(res => {
        this.users = res.filter((user: any) => user.role === 'Admin');
      });
  
      this.userStore.getUserNameFromStore().subscribe(val => {
        this.userName = val;
      });
  
      this.role = this.auth.getRole();
    }
}
