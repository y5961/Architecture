import { Component, OnInit, inject, signal, computed } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { GiftService } from '../../services/gift.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { Gift } from '../../models/GiftDTO';
import { environment } from '../../../../environment';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-gift',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gift.component.html',
  styleUrl: './gift.component.scss'
})
export class GiftComponent implements OnInit {
  imageUrl = environment.apiUrl + '/images/gift/';
  private giftService = inject(GiftService);
  private orderService = inject(OrderService);
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);
  
  gifts = signal<Gift[]>([]);
  selectedGift = signal<Gift | null>(null);
  showAuthModal = signal<boolean>(false);
  
  // סיגנלים לסינון (לשימוש מקומי או שליחה לשרת)
  categoryFilter = signal<string>('');
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);
  searchWord = signal<string>('');

  ngOnInit(): void {
    this.loadGifts();
  }

  loadGifts(): void {
    this.giftService.getAllGifts().subscribe({
      next: (data) => this.initializeGifts(data),
      error: (err) => console.error('Error loading gifts', err)
    });
  }

private initializeGifts(data: any[]): void {
  const initializedGifts = data.map(g => new Gift(g));
  this.gifts.set(initializedGifts);
}


  // onSortByPrice(): void {
  //   this.giftService.sortByPrice().subscribe({
  //     next: (data) => this.initializeGifts(data)
  //   });
  // }

  // מיון לפי פופולריות מהשרת
  onSortByPopularity(): void {
    this.giftService.sortByAmountPeople().subscribe({
      next: (data) => this.initializeGifts(data)
    });
  }

  // חיפוש לפי מילה מהשרת
  onSearchChange(event: Event): void {
    const word = (event.target as HTMLInputElement).value;
    this.searchWord.set(word);
    
    if (word.length > 1) {
      this.giftService.getByNameGift(word).subscribe({
        next: (data) => this.initializeGifts(data)
      });
    } else if (word.length === 0) {
      this.loadGifts();
    }
  }

  filterByNumOfBuyers(buyers: number): void {
    this.giftService.getByNumOfBuyers(buyers).subscribe({
      next: (data) => {
        const mapped = data.map(g => new Gift(g));
        this.gifts.set(mapped);
      }
    });
  }
  filteredGifts = computed(() => {
    return this.gifts().filter(gift => {
      const matchCategory = !this.categoryFilter() || gift.category?.name === this.categoryFilter();
      return matchCategory;
    });
  });

  // (ticket modal displayed via CartService)
  
  categories = computed(() => {
    const names = this.gifts().map(g => g.category?.name).filter(Boolean);
    return Array.from(new Set(names));
  });

  updateCategory(event: Event) {
    this.categoryFilter.set((event.target as HTMLSelectElement).value);
  }

  updateMinPrice(event: Event) {
    const val = (event.target as HTMLInputElement).value;
    this.minPrice.set(val ? parseFloat(val) : null);
  }

  updateMaxPrice(event: Event) {
    const val = (event.target as HTMLInputElement).value;
    this.maxPrice.set(val ? parseFloat(val) : null);
  }

  onSortChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    //   if (value === 'price') {
    //   this.onSortByPrice();
    // } else
   if (value === 'popularity') {
      this.onSortByPopularity();
    } else if (value === 'category') {
      // Sort by category name locally
      const sorted = [...this.gifts()].sort((a, b) => {
        const catA = a.category?.name || '';
        const catB = b.category?.name || '';
        return catA.localeCompare(catB, 'he');
      });
      this.gifts.set(sorted);
    } else {
      // No sort - reload original order
      this.loadGifts();
    }
  }

  increaseQuantity(gift: Gift): void {
    // Check if user is logged in
    if (!this.authService.isLoggedIn()) {
      this.showAuthModal.set(true);
      return;
    }

    // Block when limit reached - modal already auto-triggered by CartService effect
    if (!this.cartService.canAddGift(1)) {
      // If modal not showing yet, trigger it immediately
      if (this.cartService.remainingTickets() === 0 && !this.cartService.ticketModal()) {
        const tickets = this.cartService.totalTickets();
        const msg = tickets === 0 ? 'אין לך כרטיסים בסל — יש לרכוש חבילות כרטיסים.' : 'כל הכרטיסים שלך כבר בשימוש. יש לרכוש חבילות נוספות.';
        this.cartService.openTicketLimitModal(msg);
      }
      return;
    }

    const userId = this.authService.getUserId();
    // Debug logs to help diagnose INS UFFICIENT_TICKETS errors
    console.debug('[Gift] add request payload', { userId, giftId: gift.idGift, amount: 1 });
    console.debug('[Gift] tickets total, used, remaining', {
      totalTickets: this.cartService.totalTickets(),
      totalGifts: this.cartService.totalGiftCount(),
      remaining: this.cartService.remainingTickets(),
      packageQuantities: this.cartService.packageQuantities(),
      availablePackages: this.cartService.availablePackages()
    });

    this.orderService.addOrUpdateGiftInOrder(userId, gift.idGift, 1).subscribe({
      next: () => {
        const currentQty = this.cartService.giftQuantities()[gift.idGift] || gift.customerQuantity || 0;
        const newQty = currentQty + 1;
        const currentCart = [...this.cartService.cartGifts()];
        const idx = currentCart.findIndex(c => c.idGift === gift.idGift);
        if (idx >= 0) {
          const existing = currentCart[idx];
          currentCart[idx] = {
            ...existing,
            amount: newQty
          };
        } else {
          currentCart.push({
            idGift: gift.idGift,
            name: gift.name,
            amount: newQty,
            image: gift.image,
            category: gift.category?.name || ''
          });
        }
        this.cartService.setCartGifts(currentCart);
        this.cartService.setGiftQuantity(gift.idGift, newQty);
      },
      error: (err) => {
        try {
          const status = err?.status;
          const code = err?.error?.code || err?.error;
          if (status === 400 && (code === 'INSUFFICIENT_TICKETS' || (typeof code === 'string' && code.includes('INSUFFICIENT_TICKETS')))) {
            // Trigger modal immediately on backend rejection if not already showing
            // log useful debug info
            console.warn('[Gift] Server rejected add - INSUFFICIENT_TICKETS', {
              payload: { userId, giftId: gift.idGift, amount: 1 },
              status,
              serverError: err?.error,
              totalTickets: this.cartService.totalTickets(),
              totalGifts: this.cartService.totalGiftCount(),
              remaining: this.cartService.remainingTickets()
            });

            if (this.cartService.remainingTickets() === 0 && !this.cartService.ticketModal()) {
              const tickets = this.cartService.totalTickets();
              const msg = tickets === 0 ? 'אין לך כרטיסים בסל — יש לרכוש חבילות כרטיסים.' : 'כל הכרטיסים שלך כבר בשימוש. יש לרכוש חבילות נוספות.';
              this.cartService.openTicketLimitModal(msg);
            }
            return;
          }
        } catch (e) {
          // ignore
        }
        console.error('Error adding gift', err, {
          totalTickets: this.cartService.totalTickets(),
          totalGifts: this.cartService.totalGiftCount(),
          remaining: this.cartService.remainingTickets()
        });
      }
    });
  }

  closeLimitModal() {
    this.cartService.closeTicketLimitModal();
  }

  goToPackages() {
    this.cartService.closeTicketLimitModal();
    this.router.navigate(['/package']);
  }

  decreaseQuantity(gift: Gift): void {

    const currentQty = this.cartService.giftQuantities()[gift.idGift] || gift.customerQuantity || 0;
    if (currentQty <= 0) return;
    const userId = this.authService.getUserId();

    this.orderService.addOrUpdateGiftInOrder(userId, gift.idGift, -1).subscribe({
      next: () => {
        const newQty = currentQty - 1;
        const currentCart = [...this.cartService.cartGifts()];
        const idx = currentCart.findIndex(c => c.idGift === gift.idGift);
        if (idx >= 0) {
          if (newQty > 0) {
            currentCart[idx] = { ...currentCart[idx], amount: newQty };
          } else {
            currentCart.splice(idx, 1);
          }
        } else if (newQty > 0) {
          currentCart.push({
            idGift: gift.idGift,
            name: gift.name,
            amount: newQty,
            image: gift.image,
            category: gift.category?.name || ''
          });
        }
        this.cartService.setCartGifts(currentCart);
        this.cartService.setGiftQuantity(gift.idGift, newQty);
      }
    });
  }

getGiftQuantity(giftId: number): number {
  return this.cartService.giftQuantities()[giftId] || 0;
}

  closeAuthModal(): void {
    this.showAuthModal.set(false);
  }

  goToLogin(): void {
    this.showAuthModal.set(false);
    this.router.navigate(['/login']);
  }

  goToRegister(): void {
    this.showAuthModal.set(false);
    this.router.navigate(['/register']);
  }

  openDetails(gift: Gift): void { this.selectedGift.set(gift); }
  closeDetails(): void { this.selectedGift.set(null); }
}
