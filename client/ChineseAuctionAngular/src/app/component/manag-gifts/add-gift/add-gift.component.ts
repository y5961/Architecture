import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GiftService } from '../../../services/gift.service';
import { GiftCategoryService } from '../../../services/category.service';
import { DonorService } from '../../../services/donor.service'; // ייבוא שירות התורמים
import { GiftDTO } from '../../../models/GiftDTO';
import { GiftCategoryDTO } from '../../../models/CategoryDTO';
import { DonorDTO } from '../../../models/DonorDTO'; // הנחה שקיים DTO כזה

@Component({
  selector: 'app-add-gift',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-gift.component.html',
  styleUrl: './add-gift.component.scss'
})
export class AddGiftComponent implements OnInit {
  // הזרקת שירותים
  private giftService = inject(GiftService);
  private categoryService = inject(GiftCategoryService);
  private donorService = inject(DonorService);

  // כניסות ויציאות
  @Input() donorId: number | null = null; // אם מגיע מהורה, ננעל על תורם מסוים
  @Output() giftAdded = new EventEmitter<void>();

  // משתני מצב ונתונים
  newGift: GiftDTO = new GiftDTO();
  categories: GiftCategoryDTO[] = [];
  donors: any[] = []; // רשימת תורמים לבחירה
  
  uploadMode: 'file' | 'manual' = 'file';
  imagePreview: string | null = null;
  isUploading = false;
  isSubmitting = false;

  ngOnInit() {
    this.loadCategories();
    this.loadDonors();
    this.initDefaultValues();
  }


private initDefaultValues() {
  this.newGift.amount = 1; 
  this.newGift.categoryId = 0; 
  
  if (this.donorId && this.donorId > 0) {
    this.newGift.idDonor = this.donorId;
  } else {
    this.newGift.idDonor = 0; 
  }
}

  loadCategories() {
    this.categoryService.getAllCategories().subscribe({
      next: (data) => this.categories = data,
      error: (err) => console.error('שגיאה בטעינת קטגוריות', err)
    });
  }

  loadDonors() {
    this.donorService.getAllDonors().subscribe({
      next: (data) => {
        this.donors = data;
        // donors loaded
      },
      error: (err) => console.error('שגיאה בטעינת תורמים', err)
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    // יצירת תצוגה מקדימה מקומית (Base64)
    const reader = new FileReader();
    reader.onload = () => this.imagePreview = reader.result as string;
    reader.readAsDataURL(file);

    this.isUploading = true;
    this.giftService.uploadImage(file).subscribe({
      next: (res: any) => {
        this.newGift.image = res.fileName; // שמירת שם הקובץ שחזר מהשרת
        this.isUploading = false;
      },
      error: (err) => {
        alert("העלאת התמונה נכשלה");
        console.error(err);
        this.isUploading = false;
      }
    });
  }

  /**
   * שמירת המתנה החדשה ב-DB
   */
  saveGift() {
    // בדיקה בסיסית שכל השדות מולאו
    if (!this.newGift.idDonor || this.newGift.categoryId === 0 || !this.newGift.name || !this.newGift.amount) {
      alert("נא למלא את כל שדות החובה: שם, קטגוריה, כמות ותורם.");
      return;
    }

    this.isSubmitting = true;
    this.giftService.createGift(this.newGift).subscribe({
      next: () => {
        this.giftAdded.emit(); // מודיע לאבא לסגור מודל ולרענן רשימה
        this.resetForm();
      },
      error: (err) => {
        console.error('שגיאה 400 או אחרת:', err);
        alert("שגיאה בשמירת המתנה - וודאו שכל השדות תקינים");
        this.isSubmitting = false;
      }
    });
  }

  /**
   * איפוס הטופס לאחר הצלחה
   */
  private resetForm() {
    this.newGift = new GiftDTO();
    this.imagePreview = null;
    this.initDefaultValues();
    this.isSubmitting = false;
  }
}