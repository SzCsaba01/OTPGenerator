import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ToastService } from '../../services/toast.service';
import { Subscription } from 'rxjs';
import { IToastMessage } from '../../models/toast-message.model';

@Component({
  selector: 'app-toast',
  imports: [CommonModule],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.scss'
})
export class ToastComponent {
  messages: IToastMessage[] = [];
  private subscription = {} as Subscription;

  constructor(private toastService: ToastService) { }
  
  ngOnInit() {
    this.subscription = this.toastService.message$.subscribe(({ message, type, expiration }) => {
      if (type === 'clear') {
        this.messages = [];
      } else if (message) {
        const id = Date.now(); 
        const toastMessage = { 
          id: id,
          message: message, 
          type: type, 
          expiration: expiration 
        } as IToastMessage;
        this.messages.push(toastMessage);
        setTimeout(() => this.removeMessage(id), expiration);
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  removeMessage(id: number) {
    this.messages = this.messages.filter(msg => msg.id !== id);
  }
}
