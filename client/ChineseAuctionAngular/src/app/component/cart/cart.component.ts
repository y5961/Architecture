import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TicketLimitModalComponent } from '../shared/ticket-limit-modal/ticket-limit-modal.component';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { PackageService } from '../../services/package.service';
import { GiftService } from '../../services/gift.service';
import { Router } from '@angular/router';
import { environment } from '../../../../environment';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit {

  
  imageUrl = environment.apiUrl + '/images/packages/';
  giftImageUrl = environment.apiUrl + '/images/gift/';
  
  cartService = inject(CartService);
  private orderService = inject(OrderService);
  private authService = inject(AuthService);
  private packageService = inject(PackageService);
  private giftService = inject(GiftService);
  private router = inject(Router);
  
  cartPackages = this.cartService.cartItems;
  packageQuantities = this.cartService.packageQuantities;
  cartGifts = this.cartService.cartGifts;
  giftQuantities = this.cartService.giftQuantities;
  
  allAvailablePackages = signal<any[]>([]);
  allAvailableGifts = signal<any[]>([]);

  constructor() {}

ngOnInit() {
  this.packageService.getAllPackages().subscribe((pkgs: any[]) => {
    this.allAvailablePackages.set(pkgs);
    this.cartService.setAvailablePackages(pkgs);
    
    this.giftService.getAllGifts().subscribe((gifts: any[]) => {
      this.allAvailableGifts.set(gifts);
      
      const userId = this.authService.getUserId();
      if (userId > 0) {
        this.loadCart();
      }
    });
  });
}

  loadCart() {
  const userId = this.authService.getUserId();
  if (userId > 0) {
    this.orderService.getUserOrders(userId).subscribe({
      next: (orders: any[]) => {
        const draft = orders?.find((o: any) => o.status === 0);
        if (draft) {
          if (draft.ordersPackages) {
            const qtys: Record<number, number> = {};
            
            const enrichedPackages = draft.ordersPackages.map((item: any) => {
              const fullInfo = this.allAvailablePackages().find(p => p.idPackage === item.idPackage);
              qtys[item.idPackage] = item.quantity;
              return {
                ...item,
                idPackage: item.idPackage,
                quantity: item.quantity,
                name: fullInfo?.name ?? item.name ?? '',
                price: fullInfo?.price ?? item.price ?? 0,
                image: fullInfo?.image ?? item.image ?? '',
                amountRegular: fullInfo?.amountRegular ?? 0,
                amountPremium: fullInfo?.amountPremium ?? 0
              };
            });

            this.cartService.setAllQuantities(qtys);
            this.cartService.setCartItems(enrichedPackages);
          }

          if (draft.ordersGifts) {
            const giftQtys: Record<number, number> = {};
            
            const enrichedGifts = draft.ordersGifts.map((item: any) => {
              const candidateIds: number[] = [];
              if (item.idGift && item.idGift > 0) candidateIds.push(item.idGift);
              if (item.id && item.id > 0) candidateIds.push(item.id);
              if ((item as any).giftId && (item as any).giftId > 0) candidateIds.push((item as any).giftId);
              if (item.gift && (item.gift.idGift || item.gift.id)) {
                const nestedId = item.gift.idGift || item.gift.id;
                if (nestedId > 0) candidateIds.push(nestedId);
              }

              let fullInfo = null as any;
              if (candidateIds.length) {
                fullInfo = this.allAvailableGifts().find((g: any) => candidateIds.includes(g.idGift));
              }

              if (!fullInfo && item.name) {
                const nm = (item.name || '').toString().trim().toLowerCase();
                if (nm) fullInfo = this.allAvailableGifts().find((g: any) => (g.name || '').toString().trim().toLowerCase() === nm);
              }

              if (!fullInfo) fullInfo = undefined;

              const resolvedId = (item.idGift && item.idGift > 0) ? item.idGift : (fullInfo?.idGift || 0);
              giftQtys[resolvedId] = item.amount || 0;

              let imageToUse = '';
              if (fullInfo?.image && fullInfo.image !== 'string' && fullInfo.image.trim()) imageToUse = fullInfo.image;
              if (!imageToUse && item.image && item.image !== 'string' && (item.image + '').trim()) imageToUse = item.image;

              return {
                idGift: resolvedId,
                name: fullInfo?.name || item.name || '',
                amount: item.amount || 0,
                image: imageToUse,
                category: fullInfo?.category?.name || item.category || ''
              };
            });

            this.cartService.setAllGiftQuantities(giftQtys);
            this.cartService.setCartGifts(enrichedGifts);
          }
        }
      }
    });
  }
}



  increment(id: number) {
    this.updateQty(id, (this.packageQuantities()[id] || 0) + 1);
  }

  decrement(id: number) {
    const current = this.packageQuantities()[id] || 0;
    if (current > 0) {
      this.updateQty(id, current - 1);
    }
  }

private updateQty(idPackage: number, newQty: number) {
  const userId = this.authService.getUserId();

  // 1. קודם כל נבדוק כמה חבילות נשארו בסל כרגע (כולל הנוכחית)
  const allQtys = this.packageQuantities();
  const activePackagesCount = Object.values(allQtys).filter(qty => qty > 0).length;

  // 2. אם המשתמש לא מחובר
  if (!userId || userId <= 0) {
    this.cartService.setQuantity(idPackage, newQty);
    // מוחק רק אם זו הייתה החבילה היחידה והיא ירדה ל-0
    if (newQty === 0 && activePackagesCount <= 1) {
      this.cartService.clearCart();
    }
    return;
  }

  const prevQty = allQtys[idPackage] || 0;
  const prevCart = [...this.cartService.cartItems()];

  // 3. עדכון השרת
  this.orderService.updatePackageQuantity(userId, idPackage, newQty).subscribe({
    next: () => {
      this.cartService.setQuantity(idPackage, newQty);

      // בדיקה: האם הכמות הפכה ל-0 וגם לא נשארו חבילות אחרות בסל?
      if (newQty === 0 && activePackagesCount <= 1) {
        const confirmMsg = 'זו החבילה האחרונה בסל — האם אתה בטוח שברצונך למחוק את כל מה שיש בסל?';
        const ok = window.confirm(confirmMsg);
        
        if (!ok) {
          // המשתמש ביטל: נחזיר את המצב לקדמותו (גם בשרת)
          this.cartService.setQuantity(idPackage, prevQty);
          this.orderService.updatePackageQuantity(userId, idPackage, prevQty).subscribe();
          return;
        }

        // המשתמש אישר: ניקוי מלא
        this.cartService.clearCart();
        
        const currentGifts = [...this.cartService.cartGifts()];
        currentGifts.forEach(g => {
          const gid = g.idGift || g.id || 0;
          if (gid > 0) {
            this.orderService.removeGiftFromDraft(userId, gid).subscribe();
          }
        });

        // מחיקת הטיוטה בשרת
        this.orderService.deleteDraft(userId).subscribe();
      } else if (newQty === 0) {
        // אם זו לא החבילה האחרונה, פשוט נטען מחדש את הסל כדי שיעלם מהתצוגה
        this.loadCart();
      }
    },
    error: (err) => console.error('Error updating quantity', err)
  });
}
  incrementGift(idGift: number) {
    // Block when limit reached - modal already auto-triggered by CartService effect
    if (!this.cartService.canAddGift(1)) {
      // If modal not showing yet, trigger it immediately
      if (this.cartService.remainingTickets() === 0 && !this.cartService.ticketModal()) {
        const tickets = this.cartService.totalTickets();
        const msg = tickets === 0
          ? 'אין לך כרטיסים בסל — יש לרכוש חבילות כרטיסים.'
          : 'כל הכרטיסים שלך כבר בשימוש. יש לרכוש חבילות נוספות.';
        this.cartService.openTicketLimitModal(msg);
      }
      return;
    }
    // Send delta (+1) to API, not absolute value
    this.updateGiftQty(idGift, 1);
  }
      
decrementGift(idGift: number) {
  const current = this.giftQuantities()[idGift] || 0;
  if (current > 0) {
    // Send delta (-1) to API, not absolute value
    this.updateGiftQty(idGift, -1);
  }
}

  private updateGiftQty(idGift: number, delta: number) {
    const userId = this.authService.getUserId();
    const currentQty = this.giftQuantities()[idGift] || 0;
    const newQty = currentQty + delta;
    
    // API expects delta (relative change), not absolute quantity
    this.orderService.addOrUpdateGiftInOrder(userId, idGift, delta).subscribe({
      next: () => {
        this.cartService.setGiftQuantity(idGift, newQty);
        if (newQty === 0) {
          this.loadCart();
        }
      },
      error: (err) => {
        // If backend rejects due to insufficient tickets, show the ticket modal
        try {
          const status = err?.status;
          const code = err?.error?.code || err?.error;
          if (status === 400 && (code === 'INSUFFICIENT_TICKETS' || (typeof code === 'string' && code.includes('INSUFFICIENT_TICKETS')))) {
            // Trigger modal immediately on backend rejection if not already showing
            if (this.cartService.remainingTickets() === 0 && !this.cartService.ticketModal()) {
              const tickets = this.cartService.totalTickets();
              const msg = tickets === 0
                ? 'אין לך כרטיסים בסל — יש לרכוש חבילות כרטיסים.'
                : 'כל הכרטיסים שלך כבר בשימוש. יש לרכוש חבילות נוספות.';
              this.cartService.openTicketLimitModal(msg);
            }
            return;
          }
        } catch (e) {
          // ignore parsing errors
        }

        console.error('Error updating gift quantity', err);
      }
    });
  }

  getPackageImage(id: number): string {
    const images = ['p-green.png', 'p-blue.png', 'p-pink.png', 'p-red.png'];
    return images[id % images.length];
  }

  calculateTotal(): number {
    const packagesTotal = this.cartPackages().reduce((acc, pkg) => {
      const qty = this.packageQuantities()[pkg.idPackage] || 0;
      const price = pkg.price || 0;
      return acc + (price * qty);
    }, 0);
    return packagesTotal;
  }

getPackageGradient(id: number): string {
  const gradients = [
    'linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)',
    'linear-gradient(135deg, #a1c4fd 0%, #c2e9fb 100%)', 
    'linear-gradient(135deg, #84fb95 0%, #cef576 100%)', 
    'linear-gradient(135deg, #f6d365 0%, #fda085 100%)'  
  ];
  return gradients[id % gradients.length];
}

  proceedToBuying() {
    this.cartService.closeTicketLimitModal();
    try {
      const remaining = this.cartService.remainingTickets();
      if (remaining && remaining > 0) {
        const msg = `יש לך ${remaining} כרטיסים שלא נוצלו. האם ברצונך להמשיך לתשלום ללא מימושם?`;
        const ok = window.confirm(msg);
        if (ok) {
          this.router.navigate(['/buying']);
        } else {
          this.router.navigate(['/gift']);
        }
        return;
      }
    } catch (e) {
      console.warn('Error checking remaining tickets', e);
    }

    this.router.navigate(['/buying']);
  }
}
