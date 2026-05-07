// 1. GiftCategoryDTO Class
export class GiftCategoryDTO {
  id: number = 0;
  name: string = '';

  constructor(init?: Partial<GiftCategoryDTO>) {
    Object.assign(this, init);
  }
}

// 2. CreateGiftCategoryDTO Class
export class CreateGiftCategoryDTO {
  name: string = '';

  constructor(init?: Partial<CreateGiftCategoryDTO>) {
    Object.assign(this, init);
  }
}

// 3. UpdateGiftCategoryDTO Class
export class UpdateGiftCategoryDTO {
  name: string = '';

  constructor(init?: Partial<UpdateGiftCategoryDTO>) {
    Object.assign(this, init);
  }
}