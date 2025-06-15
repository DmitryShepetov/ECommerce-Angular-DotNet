import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NgToastModule, NgToastService, ToasterPosition, ToastType } from 'ng-angular-popup';
import { AuthResponseDto } from '../../core/interfaces/auth.interface';
import { Observable } from 'rxjs';
import { UserStoreService } from '../../core/services/user-store.service';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, CommonModule, NgToastModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  loginForm!: FormGroup;
  showPassword = false;
  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userStore: UserStoreService,
    private router: Router,
    private toast: NgToastService
  ) {
  }


    ngOnInit(): void{
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  get username(): FormControl {
    return this.loginForm.get('username') as FormControl;
  }

  get password(): FormControl {
    return this.loginForm.get('password') as FormControl;
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onLogin(){
    if(this.loginForm.valid){
      this.authService.login(this.loginForm.value).subscribe({
        next:(res: AuthResponseDto) => {
          this.toast.success('Вы авторизованы', 'Успешно', 5000);
          this.loginForm.reset();
          // this.authService.storeToken(res.accessToken);
          // const tokenPayload = this.authService.decodeToken();
          // this.userStore.setUserNameForStore(tokenPayload.name);
          // this.userStore.setRoleForStore(tokenPayload.role);
          this.router.navigate(["/profile"]);
        },
        error:(err) =>{
          this.toast.danger('Что-то пошло не так', 'Ошибка', 5000);
        }
      })
    }
    else{
      this.validateAllFormFields(this.loginForm);
    }
  }
  private validateAllFormFields(formGroup: FormGroup){
    Object.keys(formGroup.controls).forEach(field=>{
      const control = formGroup.get(field);
      if(control instanceof FormControl){
        control.markAsDirty({onlySelf: true});
      }else if(control instanceof FormGroup){
        this.validateAllFormFields(control)
      }
    })
  }
  
    onSubmit(): void {
    this.errorMessage = null;
    
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading = true;
    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.router.navigate(['/profile']);
        this.toast.success('Успешный вход', 'Вы успешно авторизованы', 5000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Неверное имя пользователя или пароль';
        this.toast.warning('Ошибка входа', "Ошибка", 5000
        );
      }
    });
  }
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

}

