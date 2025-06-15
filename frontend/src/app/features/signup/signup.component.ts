import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NgToastModule, NgToastService, ToasterPosition } from 'ng-angular-popup';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [RouterModule, ReactiveFormsModule, CommonModule, NgToastModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  type: string = "password";
  isText: boolean = false;
  singUpForm!: FormGroup;
  toasterPosition = ToasterPosition;
  showPassword: boolean = false;
  isLoading = false;

    constructor(
    private fb: FormBuilder, 
    private auth: AuthService, 
    private router: Router, 
    private toast: NgToastService
  ) {}

  ngOnInit(): void {
    this.singUpForm = this.fb.group({
      username: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(30),
        Validators.pattern(/^[a-zA-Z0-9_]+$/) // Только буквы, цифры и подчеркивание
      ]],
      firstName: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
        Validators.pattern(/^[a-zA-Zа-яА-ЯёЁ0-9]+(?: [a-zA-Zа-яА-ЯёЁ0-9]+)*$/)
      ]],
      lastName: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(50),
        Validators.pattern(/^[a-zA-Zа-яА-ЯёЁ0-9]+(?: [a-zA-Zа-яА-ЯёЁ0-9]+)*$/)
      ]],
      email: ['', [
        Validators.required,
        Validators.email
      ]],
      phone: ['', [
        Validators.required,
        Validators.pattern(/^\+?\d{6,15}$/)
      ]],
      date: ['', [
        Validators.required,
      ]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$/)
      ]],
      confirmPassword: ['', Validators.required]
    }, { 
      validator: this.passwordMatchValidator 
    });
  }

  // Валидатор для проверки совпадения паролей
  private passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
  
    if (password !== confirmPassword) {
      form.get('confirmPassword')?.setErrors({ mismatch: true });
      return { mismatch: true };
    } else {
      form.get('confirmPassword')?.setErrors(null);
      return null;
    }
  }

  hideShowPass() {
    this.isText = !this.isText;
    this.type = this.isText ? "text" : "password";
  }

  onSignup() {
    if (this.singUpForm.valid) {
      this.isLoading = true;
      
      // Удаляем confirmPassword перед отправкой
      const formValue = {
        ...this.singUpForm.value,
        date: this.formatDate(this.singUpForm.value.date)
      };
      delete formValue.confirmPassword;
      this.auth.register(formValue).subscribe({
        next: (res) => {
          this.isLoading = false;
          this.router.navigate(['/profile']);
          this.toast.success("Успешно", "Вы авторизованы", 5000);
        },
        error: (err) => {
          this.isLoading = false;
          let errorMessage = 'Что-то пошло не так';
          
          if (err.error && err.error.message) {
            errorMessage = err.error.message;
          } else if (err.status === 400) {
            errorMessage = 'Некорректные данные';
          } else if (err.status === 409) {
            errorMessage = 'Пользователь с такими данными уже существует';
          }
          
          this.toast.warning('Ошибка', errorMessage, 5000);
        }
      });
    } else {
      this.validateAllFormFields(this.singUpForm);
      this.toast.warning('Внимание', 'Пожалуйста, заполните все поля корректно', 5000);
    }
  }

  private formatDate(date: string | Date): string {
    if (typeof date === 'string') {
      // Если уже строка в правильном формате
      const d = new Date(date);
      return d.toISOString().split('T')[0];
    } else if (date instanceof Date) {
      // Если объект Date
      return date.toISOString().split('T')[0];
    }
    return '';
  }

  private validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsDirty({onlySelf: true});
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }

  // Методы для удобного доступа к полям формы
  get username() { return this.singUpForm.get('username'); }
  get firstName() { return this.singUpForm.get('firstName'); }
  get lastName() { return this.singUpForm.get('lastName'); }
  get email() { return this.singUpForm.get('email'); }
  get phone() { return this.singUpForm.get('phone'); }
  get date() { return this.singUpForm.get('date'); }
  get password() { return this.singUpForm.get('password'); }
  get confirmPassword() { return this.singUpForm.get('confirmPassword'); }
}
