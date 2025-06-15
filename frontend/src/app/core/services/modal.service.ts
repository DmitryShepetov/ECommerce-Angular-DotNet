import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ModalService {
private modalState = new Subject<number | null>();
private currentProductId: number | null = null;
modalState$ = this.modalState.asObservable();

openModal(productId: number): void {
  if (this.currentProductId !== productId) {
    this.currentProductId = productId;
    this.modalState.next(productId);
  } else {
    // Если клик по тому же продукту, сначала закрываем, затем открываем
    this.modalState.next(null);
    setTimeout(() => {
      this.modalState.next(productId);
    }, 0);
  }
}

closeModal(): void {
  this.currentProductId = null;
  this.modalState.next(null);
}
}