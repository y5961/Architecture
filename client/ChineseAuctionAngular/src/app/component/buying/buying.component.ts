import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { OrderService } from '../../services/order.service'; 
import { OrderStatus } from '../../models/OrderDTO'; 
import { AuthService } from '../../services/auth.service'; 
import { Router } from '@angular/router';
import { trigger, transition, style, animate } from '@angular/animations';
import { CartService } from '../../services/cart.service';

// PrimeNG Modules
import { InputMaskModule } from 'primeng/inputmask';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-buying',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    InputMaskModule, 
    InputTextModule, 
    ButtonModule, 
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './buying.component.html',
  styleUrls: ['./buying.component.scss'],
  animations: [
    trigger('fadeInUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('500ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class BuyingComponent implements OnInit {
  // שירותים לשימוש פנימי בלבד נשארים private
  private orderService = inject(OrderService);
  private authService = inject(AuthService); 
  private router = inject(Router); 
  private messageService = inject(MessageService);
  
  // התיקון כאן: cartService חייב להיות public כדי שה-HTML יכיר אותו
  public cartService = inject(CartService);

  paymentForm!: FormGroup; 
  isLoading: boolean = false;
  currentOrder: any = null; 
  isCompleted: boolean = false;

  ngOnInit() {
    this.initForm();
    this.loadDraftOrder();
  }

  initForm() {
    this.paymentForm = new FormGroup({
      cardNumber: new FormControl('', [Validators.required, Validators.pattern(/^\d{4}-\d{4}-\d{4}-\d{4}$/)]),
      expiry: new FormControl('', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]),
      cvv: new FormControl('', [Validators.required, Validators.pattern(/^\d{3}$/)]),
      holderName: new FormControl('', Validators.required)
    });
  }

  loadDraftOrder() {
    const userId = this.authService?.getUserId();
    if (!userId) return;

    this.orderService.getUserOrders(userId).subscribe({
      next: (res: any) => {
        const ordersArray = Array.isArray(res) ? res : res?.orders;
        if (ordersArray && ordersArray.length > 0) {
          const draft = ordersArray.find((o: any) => o.status === OrderStatus.Draft);
          this.currentOrder = draft;
          
          if (!this.currentOrder) {
             const lastOrder = ordersArray[0];
             if (lastOrder.status === OrderStatus.Completed) {
                this.currentOrder = lastOrder;
                this.isCompleted = true;
             }
          }
        }
      },
      error: (err) => console.error('Error loading orders:', err)
    });
  }

  onCompleteOrder() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      this.messageService.add({severity:'warn', summary:'פרטים חסרים', detail:'נא למלא את כל שדות האשראי'});
      return;
    }

    const orderId = this.currentOrder?.idOrder;
    if (!orderId) return;

    this.isLoading = true;
    this.orderService.completeOrder(orderId).subscribe({
      next: () => {
        this.isLoading = false;
        this.isCompleted = true;
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 200) {
          this.isCompleted = true;
        } else {
          this.messageService.add({severity:'error', summary:'שגיאה', detail:'חלה שגיאה בסגירת ההזמנה'});
        }
      }
    });
  }

  goToHome() {
    this.router.navigate(['/']); 
  }
}