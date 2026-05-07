import { Gift } from './GiftDTO';

// PackageDTO - עבור תצוגה
export class PackageDTO {
  idPackage: number = 0;
  amountRegular: number = 0;
  amountPremium?: number;
  price: number = 0;
  name: string = '';
  description?: string;

  constructor(init?: Partial<PackageDTO>) {
    Object.assign(this, init);
  }
}

export class PackageCreateDTO {
  amountRegular: number = 0;
  amountPremium?: number;
  price: number = 0;
  name: string = '';
  description?: string;

  constructor(init?: Partial<PackageCreateDTO>) {
    Object.assign(this, init);
  }
}

export class OrdersPackageDTO {
  idOrdersPackage: number = 0;
  idPackage: number = 0;
  idOrder: number = 0;
  // הוסיפי שדות נוספים אם קיימים ב-API (כמו כמות/מחיר)
  constructor(init?: Partial<OrdersPackageDTO>) {
    Object.assign(this, init);
  }
}

// Package Model - המודל המלא הכולל קשרים
export class Package {
  idPackage: number = 0;
  amountRegular: number = 0;
  amountPremium?: number;
  price: number = 0;
  name: string = '';
  description?: string;
  gifts: Gift[] = []; // הוחלף מ-any
  ordersPackage: OrdersPackageDTO[] = [];

  constructor(init?: Partial<Package>) {
    Object.assign(this, init);
  }
}