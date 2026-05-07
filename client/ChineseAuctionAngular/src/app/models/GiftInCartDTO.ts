import { Gift } from './GiftDTO';
import { OrderDTO } from './OrderDTO';

export class OrdersGift {
  idOrdersGift: number = 0;
  idGift: number = 0;
  gift?: Gift; // קשר למודל המתנה
  idOrder: number = 0;
  order?: OrderDTO; // קשר למודל ההזמנה
  amount: number = 0;

  constructor(init?: Partial<OrdersGift>) {
    Object.assign(this, init);
  }
}