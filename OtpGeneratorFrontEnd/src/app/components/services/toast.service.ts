import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private messageSubject = new Subject<{ message: string, type: 'success' | 'error' | 'clear', expiration: number}>();

  message$ = this.messageSubject.asObservable();

  showMessage(message: string, type: 'success' | 'error', expiration = 5000) {
    this.messageSubject.next({ message, type, expiration });
  }

  clearMessages() {
    this.messageSubject.next({ message: '', type: 'clear', expiration: 0 });
  }
}
