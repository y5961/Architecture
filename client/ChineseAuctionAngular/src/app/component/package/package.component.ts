// import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PackageService } from '../../services/package.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { PackageDTO } from '../../models/PackageDTO';
import { Router } from '@angular/router';
import { environment } from '../../../../environment';
import { Component, inject, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-package',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './package.component.html',
  styleUrl: './package.component.scss'
})
export class PackageComponent implements OnInit {
  imageUrl = environment.apiUrl + '/images/packages/';
  private packageService = inject(PackageService);
  private orderService = inject(OrderService);
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);

  packages = signal<PackageDTO[]>([]);
  packageQuantities = this.cartService.packageQuantities;
  showAuthModal = signal<boolean>(false);

  ngOnInit() {
    this.packageService.getAllPackages().subscribe((data: PackageDTO[]) => {
      this.packages.set(data);
      // Keep CartService in sync so other pages can compute total tickets
      this.cartService.setAvailablePackages(data as any[]);
    });

    this.loadUserData();
  }

  loadUserData() {
    const userId = this.authService.getUserId();
    if (userId > 0) {
      this.orderService.getUserOrders(userId).subscribe({
        next: (userResponse: any) => {
          const draft = userResponse.orders?.find((o: any) => o.status === 0);
          if (draft) {
            const qtys: Record<number, number> = {};
            draft.ordersPackages.forEach((p: any) => {
              qtys[p.idPackage] = p.quantity;
            });
            this.cartService.setAllQuantities(qtys);
            this.cartService.setCartItems(draft.ordersPackages);
          }
        }
      });
    }
  }
handlePackageUpdate(packageId: number, action: 'increment' | 'decrement') {
  // Check if user is logged in
  if (!this.authService.isLoggedIn()) {
    this.showAuthModal.set(true);
    return;
  }

  const currentQty = this.packageQuantities()[packageId] || 0;
  const newQty = action === 'increment' ? currentQty + 1 : currentQty - 1;

  if (newQty < 0) return;

    // Optimistic update: take snapshots and apply locally first
    const prevQty = currentQty;
    const prevCart = [...this.cartService.cartItems()];

    this.cartService.setQuantity(packageId, newQty);

  // update cartItems so Cart shows change immediately
  const pkgInfo = this.packages().find(p => p.idPackage === packageId) || { idPackage: packageId, price: 0 };
  const updatedCart = [...this.cartService.cartItems()];
  const idx = updatedCart.findIndex((c: any) => c.idPackage === packageId);
  if (newQty > 0) {
    const item = {
      idPackage: packageId,
      quantity: newQty,
      price: pkgInfo.price || 0,
      amountRegular: (pkgInfo as any).amountRegular ?? 0,
      amountPremium: (pkgInfo as any).amountPremium ?? 0,
      name: (pkgInfo as any).name || ''
    };
    if (idx >= 0) updatedCart[idx] = { ...updatedCart[idx], quantity: newQty, price: pkgInfo.price || updatedCart[idx].price };
    else updatedCart.push(item);
  } else {
    if (idx >= 0) updatedCart.splice(idx, 1);
  }
  this.cartService.setCartItems(updatedCart);

  const totalPackages = Object.values(this.cartService.packageQuantities()).reduce((acc, v) => acc + (v || 0), 0);
  if (totalPackages === 0) {
    // When packages drop to zero, clear the entire cart locally
    this.cartService.clearCart();
    const userIdConfirm = this.authService.getUserId();
    if (userIdConfirm && userIdConfirm > 0) {
      // Prefer deleting server draft; if that fails, attempt to set this package quantity=0
      this.orderService.deleteDraft(userIdConfirm).subscribe({
        next: () => {},
        error: (err) => {
          console.warn('deleteDraft failed, attempting to update package to 0 as fallback', err);
          // best-effort fallback to ensure server draft won't repopulate the cart
          this.orderService.updatePackageQuantity(userIdConfirm, packageId, 0).subscribe({
            next: () => {},
            error: (err2) => console.warn('Fallback updatePackageQuantity failed', err2)
          });
        }
      });
    }
    return;
  }

  const userId = this.authService.getUserId();
  if (!userId || userId <= 0) {
    // not logged in - keep local change only
    return;
  }

  // confirm change with server, revert on error
  this.orderService.updatePackageQuantity(userId, packageId, newQty).subscribe({
    next: () => {
      // success - nothing to do
    },
    error: (err) => {
      // revert optimistic update
      this.cartService.setQuantity(packageId, prevQty);
      this.cartService.setCartItems(prevCart);
      console.error('Failed updating package quantity', err);
    }
  });
}

getPackageGradient(id: number): string {
    const gradients = [
      'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', // ירוק
      'linear-gradient(135deg, #00c6fb 0%, #005bea 100%)', // כחול
      'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', // ורוד-אדום
      'linear-gradient(135deg, #f6d365 0%, #fda085 100%)'  // כתום
    ];
    // אנחנו משתמשים ב-Modulo (%) כדי שאם יש יותר מ-4 חבילות, הצבעים יחזרו על עצמם
    return gradients[id % gradients.length];
  }

  getPackageImage(id: number): string {
    const images = ['p-green.png', 'p-blue.png', 'p-pink.png', 'p-red.png'];
    return images[id % images.length];
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
}

