import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthResponseDto } from '../interfaces/auth.interface';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})

export class UserStoreService {
  private username$ = new BehaviorSubject<string>("");
  private role$ = new BehaviorSubject<string>("");
  private phone$ = new BehaviorSubject<string>("");

  constructor() {}

  public getRoleFromStore(){
    return this.role$.asObservable();
  }
  public setRoleForStore(role: string) {
    this.role$.next(role);
  }
  public getUserNameFromStore(){
    return this.username$.asObservable();
  }
  public setUserNameForStore(username :string){
    this.username$.next(username);
  }
  public getPhoneNameForStore(){
    return this.phone$.asObservable();
  }
}

