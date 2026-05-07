import { Component, Input, Output, EventEmitter } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { Gift } from '../../../models/GiftDTO';

@Component({
  selector: 'app-donor-gifts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './donor-gifts.component.html',
  styleUrl: './donor-gifts.component.scss'
})

export class DonorGiftsComponent {
  @Input() gifts: Gift[] = [];
  @Input() donorName: string = '';
  @Input() imageBaseUrl: string = '';
  @Output() deleteGift = new EventEmitter<number>();
  giftToDelete: any = null;

  onDelete(giftId: number | undefined) {
    if (giftId !== undefined && confirm('האם את בטוחה שברצונך למחוק את המתנה?')) {
      this.deleteGift.emit(giftId);
    }
  }

openConfirm(gift: any) {
  this.giftToDelete = gift;
}

confirmDelete() {
  if (this.giftToDelete) {
    this.deleteGift.emit(this.giftToDelete.idGift);
    this.giftToDelete = null; // סגירת הדיאלוג
  }
}
}



