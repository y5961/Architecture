import { OrderDTO } from "./OrderDTO";

// 1. UserDTO Class
export class UserDTO {
  phoneNumber: string = '';
  firstName: string = '';
  lastName: string = '';
  city?: string = '';
  address?: string = '';
  email?: string = '';
  identity: string = '';

  constructor(init?: Partial<UserDTO>) {
    Object.assign(this, init);
  }
}

// 2. DtoLogin Class (לשימוש ברישום מורחב)
export class DtoLogin {
  password?: string = '';
  identity: string = '';
  firstName: string = '';
  lastName: string = '';
  email?: string = '';
  phoneNumber: string = '';
  city: string = '';
  address: string = '';

  constructor(init?: Partial<DtoLogin>) {
    Object.assign(this, init);
  }
}

// 3. DtologinRequest Class (עבור מסך התחברות)
export class DtoLoginRequest {
  email: string = '';
  password: string = '';

  constructor(init?: Partial<DtoLoginRequest>) {
    Object.assign(this, init);
  }
}

// 4. DtoLoginResponse Class
export class DtoLoginResponse {
  token: string = '';
  user: UserDTO = new UserDTO();

  constructor(init?: Partial<DtoLoginResponse>) {
    Object.assign(this, init);
  }
}

// 5. DtoUserOrder Class
export class DtoUserOrder {
  firstName: string = '';
  lastName: string = '';
  orders: OrderDTO[] = []; // הוחלף מ-any[]

  constructor(init?: Partial<DtoUserOrder>) {
    Object.assign(this, init);
  }
}