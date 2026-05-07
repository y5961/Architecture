import { Injectable, signal, computed, effect, inject, Injector } from '@angular/core';
import { OrderService } from './order.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private injector = inject(Injector);

  constructor() {
    effect(() => {
      const remaining = this.remainingTickets();
      const isModalOpen = this.ticketModalSignal();
      if (isModalOpen && remaining > 0) {
        this.closeTicketLimitModal();
      }
    });
  }

  private quantitiesSignal = signal<Record<number, number>>({});
  private cartItemsSignal = signal<any[]>([]);
  private giftQuantitiesSignal = signal<Record<number, number>>({});
  private cartGiftsSignal = signal<any[]>([]);
  private availablePackagesSignal = signal<any[]>([]);
  private ticketModalSignal = signal<boolean>(false);
  private ticketLimitMessageSignal = signal<string>('');
  private ticketModalSuppressedSignal = signal<boolean>(false);

  packageQuantities = this.quantitiesSignal.asReadonly();
  cartItems = this.cartItemsSignal.asReadonly();
  giftQuantities = this.giftQuantitiesSignal.asReadonly();
  cartGifts = this.cartGiftsSignal.asReadonly();
  availablePackages = this.availablePackagesSignal.asReadonly();
  ticketModal = this.ticketModalSignal.asReadonly();
  ticketMessage = this.ticketLimitMessageSignal.asReadonly();

  totalGiftCount = computed(() => {
    return Object.values(this.giftQuantitiesSignal()).reduce((acc, v) => acc + (v || 0), 0);
  });

  totalTickets = computed(() => {
    const pkgs = this.availablePackagesSignal();
    const quantities = this.quantitiesSignal();
    return Object.keys(quantities).reduce((acc, idStr) => {
      const id = Number(idStr);
      const pkg = pkgs.find(p => p.idPackage === id);
      if (pkg) {
        const regular = pkg.amountRegular || 0;
        const premium = pkg.amountPremium || 0;
        return acc + ((regular + premium) * quantities[id]);
      }
      return acc;
    }, 0);
  });

  remainingTickets = computed(() => {
    return Math.max(0, this.totalTickets() - this.totalGiftCount());
  });

  private totalPriceSignal = computed(() => {
    const pkgs = this.availablePackagesSignal();
    const quantities = this.quantitiesSignal();
    return Object.keys(quantities).reduce((acc, idStr) => {
      const id = Number(idStr);
      const pkg = pkgs.find(p => p.idPackage === id || (p as any).id === id);
      const qty = quantities[id] || 0;
      const price = pkg ? (pkg.price || 0) : 0;
      return acc + (price * qty);
    }, 0);
  });

  totalPrice(): number { return this.totalPriceSignal(); }

  canAddGift(n: number = 1): boolean {
    const totalAllowed = this.totalTickets();
    const alreadyUsed = this.totalGiftCount();
    return totalAllowed > 0 && (alreadyUsed + n) <= totalAllowed;
  }

  ticketLimitMessage(n: number = 1): string {
    const tickets = this.totalTickets();
    if (tickets <= 0) return 'אין לך כרטיסים בסל — יש לרכוש חבילות כרטיסים.';
    const missing = (this.totalGiftCount() + n) - tickets;
    return `אין מספיק כרטיסים. חסרים ${missing} כרטיסים.`;
  }

  openTicketLimitModal(message: string) {
    this.ticketLimitMessageSignal.set(message);
    this.ticketModalSignal.set(true);
  }

  closeTicketLimitModal(manual: boolean = false) {
    this.ticketModalSignal.set(false);
    if (manual) {
      this.ticketModalSuppressedSignal.set(true);
      setTimeout(() => this.ticketModalSuppressedSignal.set(false), 10000);
    }
  }

  setQuantity(packageId: number, qty: number) {
    this.quantitiesSignal.update(current => {
      const updated = { ...current };
      if (qty <= 0) delete updated[packageId];
      else updated[packageId] = qty;
      return updated;
    });
    this.enforceGiftLimit(); 
  }

  setAllQuantities(quantities: Record<number, number>) {
    this.quantitiesSignal.set(quantities);
    this.enforceGiftLimit();
  }

  private enforceGiftLimit() {
    const allowed = this.totalTickets(); 
    const giftQtys = { ...this.giftQuantitiesSignal() }; 
    let currentTotal = Object.values(giftQtys).reduce((acc, v) => acc + (v || 0), 0);

    if (currentTotal <= allowed) return;

    const giftsList = [...this.cartGiftsSignal()];
    const removals: { id: number, amount: number, isDelete: boolean }[] = [];

    for (let i = giftsList.length - 1; i >= 0 && currentTotal > allowed; i--) {
      const g = giftsList[i];
      const id = g.idGift;
      const qty = giftQtys[id] || 0;
      if (qty <= 0) continue;

      const excess = currentTotal - allowed;
      const removeCount = Math.min(qty, excess);
      const newQty = qty - removeCount;
      
      currentTotal -= removeCount;

      if (newQty > 0) {
        giftQtys[id] = newQty;
        giftsList[i] = { ...g, amount: newQty };
        removals.push({ id, amount: removeCount, isDelete: false });
      } else {
        delete giftQtys[id];
        giftsList.splice(i, 1);
        removals.push({ id, amount: qty, isDelete: true });
      }
    }

    this.giftQuantitiesSignal.set(giftQtys);
    this.cartGiftsSignal.set(giftsList);
    this.syncRemovalsWithServer(removals);
  }

  private syncRemovalsWithServer(removals: { id: number, amount: number, isDelete: boolean }[]) {
    if (removals.length === 0) return;

    const orderService = this.injector.get(OrderService);
    const authService = this.injector.get(AuthService);
    const userId = authService.getUserId();

    if (userId > 0) {
      removals.forEach(rem => {
        if (rem.isDelete) {
          orderService.removeGiftFromDraft(userId, rem.id).subscribe();
        } else {
          orderService.addOrUpdateGiftInOrder(userId, rem.id, -rem.amount).subscribe();
        }
      });
    }
  }

  setGiftQuantity(giftId: number, qty: number) {
    this.giftQuantitiesSignal.update(current => {
      const updated = { ...current };
      if (qty <= 0) delete updated[giftId];
      else updated[giftId] = qty;
      return updated;
    });
  }

  setCartItems(items: any[]) { this.cartItemsSignal.set(items); }
  setAvailablePackages(items: any[]) { this.availablePackagesSignal.set(items); }
  setCartGifts(gifts: any[]) { this.cartGiftsSignal.set(gifts); }
  setAllGiftQuantities(quantities: Record<number, number>) { this.giftQuantitiesSignal.set(quantities); }

  clearCart() {
    this.quantitiesSignal.set({});
    this.cartItemsSignal.set([]);
    this.giftQuantitiesSignal.set({});
    this.cartGiftsSignal.set([]);
  }
}