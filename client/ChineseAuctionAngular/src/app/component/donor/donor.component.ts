import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DonorService } from '../../services/donor.service';
import { DonorDTO, DonorCreateDTO } from '../../models/DonorDTO';
import { Gift } from '../../models/GiftDTO';
import { environment } from '../../../../environment';
import { Subject, debounceTime, distinctUntilChanged, Observable } from 'rxjs';
import { DonorGiftsComponent } from './donor-gifts/donor-gifts.component';
import { GiftService } from '../../services/gift.service';
import { AddGiftComponent } from './add-gift/add-gift.component';

@Component({
  selector: 'app-donor',
  standalone: true,
  imports: [CommonModule, FormsModule, DonorGiftsComponent, AddGiftComponent],
  templateUrl: './donor.component.html',
  styleUrl: './donor.component.scss'
})
export class DonorComponent implements OnInit {
  private donorService = inject(DonorService);
  private giftService = inject(GiftService);

  // --- Signals State ---
  donors = signal<DonorDTO[]>([]);
  donorGifts = signal<{ [key: number]: Gift[] }>({});
  expandedDonorId = signal<number | null>(null);
  addingGiftToDonorId = signal<number | null>(null);
  isLoading = signal<boolean>(false);
  searchQuery = signal<string>('');
  currentFilter = signal<'name' | 'email' | 'gift'>('name');

  // --- Computed Signals ---
  activeFilterName = computed(() => {
    const names = { name: 'שם', email: 'אימייל', gift: 'מתנה' };
    return names[this.currentFilter()];
  });

  // --- Modals & Utils ---
  showAddModal = false;
  showEditModal = false;
  donorToDelete: DonorDTO | null = null;
  newDonor: DonorCreateDTO = new DonorCreateDTO();
  editingDonor: DonorDTO | null = null;
  imageBaseUrl = `${environment.apiUrl}/images/gift/`;
  deleteError: string | null = null;

  private searchSubject = new Subject<string>();

  ngOnInit() {
    this.loadDonors();
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(query => this.executeSearch(query));
  }

  loadDonors() {
    this.isLoading.set(true);
    this.donorService.getAllDonors().subscribe({
      next: (data: DonorDTO[]) => {
        this.donors.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  // --- CRUD Operations ---

  submitAddDonor() {
    if (!this.newDonor.firstName || !this.newDonor.lastName) return;
    
    this.donorService.createDonor(this.newDonor).subscribe({
      next: (id) => {
        const addedDonor = new DonorDTO({ idDonor: id, ...this.newDonor });
        this.donors.update(list => [...list, addedDonor]);
        this.showAddModal = false;
        this.newDonor = new DonorCreateDTO(); // Reset
      }
    });
  }

  submitEditDonor() {
    if (this.editingDonor) {
      this.donorService.updateDonor(this.editingDonor.idDonor, new DonorCreateDTO(this.editingDonor)).subscribe(() => {
        this.donors.update(list => 
          list.map(d => d.idDonor === this.editingDonor?.idDonor ? this.editingDonor! : d)
        );
        this.showEditModal = false;
      });
    }
  }

  executeDelete() {
    if (this.donorToDelete) {
      this.donorService.deleteDonor(this.donorToDelete.idDonor).subscribe(() => {
        this.donors.update(list => list.filter(d => d.idDonor !== this.donorToDelete?.idDonor));
        this.donorToDelete = null;
      });
    }
  }

  // --- Gift Logic ---

  toggleGifts(donor: DonorDTO) {
    const id = donor.idDonor;
    if (this.expandedDonorId() === id) {
      this.expandedDonorId.set(null);
      return;
    }
    this.expandedDonorId.set(id);
    this.addingGiftToDonorId.set(null);

    if (!this.donorGifts()[id]) {
      this.donorService.getGiftsByDonorId(donor.firstName).subscribe(gifts => {
        this.donorGifts.update(prev => ({ ...prev, [id]: gifts || [] }));
      });
    }
  }

  confirmAddGift(donor: DonorDTO) {
    const currentId = this.addingGiftToDonorId();
    this.addingGiftToDonorId.set(currentId === donor.idDonor ? null : donor.idDonor);
    this.expandedDonorId.set(null);
  }

  onGiftAddedSuccessfully(donorId: number) {
    this.addingGiftToDonorId.set(null);
    const donor = this.donors().find(d => d.idDonor === donorId);
    if (donor) {
      this.donorService.getGiftsByDonorId(donor.firstName).subscribe(gifts => {
        this.donorGifts.update(prev => ({ ...prev, [donorId]: gifts || [] }));
        this.expandedDonorId.set(donorId);
      });
    }
  }

  deleteGiftFromDonor(giftId: number, donorId: number) {
    this.giftService.deleteGift(giftId).subscribe({
      next: () => {
        this.donorGifts.update(prev => {
          const updatedGifts = prev[donorId]?.filter(g => g.idGift !== giftId) || [];
          return { ...prev, [donorId]: updatedGifts };
        });
      }
    });
  }

  // --- Search & Filter ---

  executeSearch(query: string) {
    if (!query.trim()) {
      this.loadDonors();
      return;
    }
    this.isLoading.set(true);
    let obs: Observable<any>;
    switch (this.currentFilter()) {
      case 'email': obs = this.donorService.sortByEmail(query); break;
      case 'gift': obs = this.donorService.sortByGift(query); break;
      default: obs = this.donorService.sortByName(query); break;
    }
    obs.subscribe({
      next: (data) => {
        this.donors.set(Array.isArray(data) ? data : (data ? [data] : []));
        this.isLoading.set(false);
      },
      error: () => { this.donors.set([]); this.isLoading.set(false); }
    });
  }

  onSearchInput() {
    this.searchSubject.next(this.searchQuery());
  }

  setFilter(filter: 'name' | 'email' | 'gift', name: string) {
    this.currentFilter.set(filter);
    if (this.searchQuery()) this.executeSearch(this.searchQuery());
  }

  clearSearch() {
    this.searchQuery.set('');
    this.loadDonors();
  }

  openEditModal(donor: DonorDTO) {
    this.editingDonor = { ...donor };
    this.showEditModal = true;
  }

  confirmDelete(donor: DonorDTO) {
    this.deleteError = null;
    // Check if donor has purchased gifts
    this.donorService.getGiftsByDonorId(donor.firstName).subscribe({
      next: (gifts) => {
        // Check if any of the donor's gifts have been purchased
        const hasPurchasedGifts = gifts && gifts.some((gift: any) => 
          gift.totalPurchases > 0 || gift.customerQuantity > 0 || (gift.ordersGifts && gift.ordersGifts.length > 0)
        );
        
        if (hasPurchasedGifts) {
          this.deleteError = `לא ניתן למחוק את התורם ${donor.firstName} ${donor.lastName} מכיוון שלחלק מהמתנות שלו כבר יש רוכשים.`;
          this.donorToDelete = null;
        } else {
          this.donorToDelete = donor;
        }
      },
      error: () => {
        // If we can't fetch gifts, allow the deletion to proceed
        this.donorToDelete = donor;
      }
    });
  }
}