import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagGiftsComponent } from './manag-gifts.component';

describe('ManagGiftsComponent', () => {
  let component: ManagGiftsComponent;
  let fixture: ComponentFixture<ManagGiftsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagGiftsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagGiftsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
