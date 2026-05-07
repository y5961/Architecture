export enum OrderStatus {
  Draft ,
  Completed 
}

export class OrdersGiftDTO {
  category: string = '';
  name: string = '';
  amount: number = 1;
  description: string = '';
  image?: string;

  constructor(init?: Partial<OrdersGiftDTO>) {
    Object.assign(this, init);
  }
}

export class OrdersPackageDTO {
  idPackage: number = 0;
  quantity: number = 0;
  name: string = '';
  price: number = 0;

  constructor(init?: Partial<OrdersPackageDTO>) {
    Object.assign(this, init);
  }
}

export class OrderDTO {
  idOrder?: number; 
  
  totalAmount: number = 0; 
  totalPrice: number = 0;
  idUser: number = 0;     
  amount: number = 1;     
  orderDate: Date = new Date(); 
  status: OrderStatus = OrderStatus.Draft; 
  ordersPackages: OrdersPackageDTO[] = []; 
  ordersGifts: OrdersGiftDTO[] = [];

  constructor(init?: Partial<OrderDTO>) {
    Object.assign(this, init);
    if (init?.orderDate) {
      this.orderDate = new Date(init.orderDate);
    }
    this.ordersPackages = init?.ordersPackages ? init.ordersPackages.map(p => new OrdersPackageDTO(p)) : [];
    this.ordersGifts = init?.ordersGifts ? init.ordersGifts.map(g => new OrdersGiftDTO(g)) : [];
  }
}