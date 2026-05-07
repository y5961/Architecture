import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { DonorService } from '../../../services/donor.service';
import { GiftService } from '../../../services/gift.service';
import { GiftCategoryService } from '../../../services/category.service';
import { Gift, GiftDTO } from '../../../models/GiftDTO';
import { Donor } from '../../../models/DonorDTO';
import { GiftCategoryDTO } from '../../../models/CategoryDTO';

@Component({
  selector: 'app-edit-gift-modal',
  standalone: true,
  imports: [
    CommonModule, FormsModule, DialogModule, DropdownModule, 
    InputTextModule, InputNumberModule, ButtonModule
  ],
  templateUrl: './edit-gift.component.html',
  styleUrl: './edit-gift.component.scss'    // הוסיפי את השורה הזו אם יש לך קובץ עיצוב
})
export class EditGiftComponent implements OnInit, OnChanges {
  private donorService = inject(DonorService);
  private categoryService = inject(GiftCategoryService);
  private giftService = inject(GiftService);

  @Input() visible = false;
  @Input() gift: Gift | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<any>();

  donors = signal<Donor[]>([]);
  categories = signal<GiftCategoryDTO[]>([]);
  
  editForm: any = {};
  imagePreview: string | null = null;
  isUploading = false;

  ngOnInit() {
    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['gift'] && this.gift) {
      // מעתיק את האובייקט כדי לא לשנות את המקור לפני שמירה
      this.editForm = { ...this.gift };
      this.imagePreview = null; 
    }
  }

  loadData() {
    this.donorService.getAllDonors().subscribe(data => this.donors.set(data));
    this.categoryService.getAllCategories().subscribe(data => this.categories.set(data));
  }

  onClose() {
    this.imagePreview = null;
    this.close.emit();
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    // תצוגה מקדימה מקומית
    const reader = new FileReader();
    reader.onload = () => this.imagePreview = reader.result as string;
    reader.readAsDataURL(file);

    this.isUploading = true;
    this.giftService.uploadImage(file).subscribe({
      next: (res: any) => {
        this.editForm.image = res.fileName; // מעדכן את שם הקובץ בטופס
        this.isUploading = false;
      },
      error: (err) => {
        console.error('העלאת תמונה נכשלה', err);
        this.isUploading = false;
        alert('נכשל בהעלאת התמונה');
      }
    });
  }

  save() {
    if (this.editForm && this.editForm.idGift) {
      
      const dataToSend: GiftDTO = {
        idGift: this.editForm.idGift,
        name: this.editForm.name,
        description: this.editForm.description,
        categoryId: this.editForm.idGiftCategory || this.editForm.categoryId,
        amount: this.editForm.amount || 1, 
        image: this.editForm.image,
        idDonor: this.editForm.idDonor
      };

      this.giftService.updateGift(dataToSend.idGift, dataToSend).subscribe({
        next: () => {
          console.log('עריכה הצליחה!');
          this.saved.emit(this.editForm);
          this.onClose();
        },
        error: (err) => {
          console.error('שגיאה 400 - בדקי תאימות שדות ב-Network Tab', err);
        }
      });
    }
  }
}

