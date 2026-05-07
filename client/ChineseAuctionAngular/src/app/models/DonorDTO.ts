import { Gift } from './GiftDTO';

export class DonorDTO {
  idDonor: number = 0;
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  phoneNumber: string = '';

  constructor(init?: Partial<DonorDTO>) {
    Object.assign(this, init);
  }
}

export class DonorCreateDTO {
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  phoneNumber: string = '';

  constructor(init?: Partial<DonorCreateDTO>) {
    Object.assign(this, init);
  }
}

export class Donor {
  idDonor: number = 0;
  firstName: string = '';
  lastName: string = '';
  email: string = '';
  phoneNumber: string = '';
  gifts?: Gift[] = []; 

  constructor(init?: Partial<Donor>) {
    Object.assign(this, init);
  }
}