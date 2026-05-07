import { Package } from './PackageDTO';
import { OrderDTO } from './OrderDTO';

export class OrdersPackage {
  idPackageOrder: number = 0;
  orderId: number = 0;
  order?: OrderDTO;       
  idPackage: number = 0;
  package?: Package;      
  quantity: number = 1;
  priceAtPurchase: number = 0;

  constructor(init?: Partial<OrdersPackage>) {
    Object.assign(this, init);
  }
}

