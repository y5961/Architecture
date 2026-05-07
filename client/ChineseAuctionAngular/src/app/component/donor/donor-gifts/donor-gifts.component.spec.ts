import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorGiftsComponent } from './donor-gifts.component';

describe('DonorGiftsComponent', () => {
  let component: DonorGiftsComponent;
  let fixture: ComponentFixture<DonorGiftsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorGiftsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DonorGiftsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
