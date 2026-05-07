import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Donor, DonorDTO, DonorCreateDTO } from '../models/DonorDTO';
import { Gift } from '../models/GiftDTO';
import { environment } from '../../../environment';

@Injectable({
  providedIn: 'root'
})
export class DonorService {
  readonly BASE_URL = `${environment.apiUrl}/api/Donors`;
  private http = inject(HttpClient);

  constructor() {}

  getAllDonors(): Observable<DonorDTO[]> {
    return this.http.get<DonorDTO[]>(`${this.BASE_URL}`);
  }

  getDonorById(id: number): Observable<DonorDTO> {
    return this.http.get<DonorDTO>(`${this.BASE_URL}/${id}`);
  }

  createDonor(dto: DonorCreateDTO): Observable<number> {
    return this.http.post<number>(`${this.BASE_URL}`, dto);
  }

  updateDonor(id: number, dto: DonorCreateDTO): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/${id}`, dto);
  }

  deleteDonor(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/${id}`);
  }

getGiftsByDonorId(donorName: string): Observable<Gift[]> { 
  return this.http.get<Gift[]>(`api/Gift/sort/donor/${donorName}`);
}

sortByName(name: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.BASE_URL}/sort/name/${name}`);
}

sortByEmail(email: string): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.BASE_URL}/sort/email/${email}`);
}

sortByGift(giftName: string): Observable<Donor> {
  return this.http.get<Donor>(`${this.BASE_URL}/sort/gift/${giftName}`);
}

hasPurchasedGifts(donorId: number): Observable<boolean> {
  return this.http.get<boolean>(`${this.BASE_URL}/${donorId}/has-purchased-gifts`);
}

}