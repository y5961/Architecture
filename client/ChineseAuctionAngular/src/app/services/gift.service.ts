import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Gift, GiftDTO, GiftNewDTO } from '../models/GiftDTO';
import { Observable } from 'rxjs';
import { environment } from '../../../environment';
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class GiftService {
  readonly BASE_URL = `${environment.apiUrl}/api/Gift`; 
  private http = inject(HttpClient);

  constructor() {}

  getAllGifts(): Observable<Gift[]> {
    return this.http.get<Gift[]>(this.BASE_URL);
  }

  getGiftById(id: number): Observable<Gift> {
    return this.http.get<Gift>(`${this.BASE_URL}/${id}`);
  }

  createGift(dto: GiftDTO): Observable<Gift> {
    return this.http.post<Gift>(this.BASE_URL, dto);
  }

  updateGift(id: number, gift: GiftDTO): Observable<any> {
    return this.http.put(`${this.BASE_URL}/${id}`, gift);
  }

  deleteGift(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.BASE_URL}/${id}`);
  }

  drawWinnerForGift(giftId: number): Observable<any> {
    return this.http.post<any>(`${this.BASE_URL}/${giftId}/draw`, {});
  }

  getParticipantsByGiftId(giftId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.BASE_URL}/${giftId}/participants`);
  }

  getByNameGift(word: string): Observable<Gift[]> {
    return this.http.get<Gift[]>(`${this.BASE_URL}/sort/word/${word}`);
  }

  getByNameDonor(donor: string): Observable<Gift[]> {
    return this.http.get<Gift[]>(`${this.BASE_URL}/sort/donor/${donor}`);
  }

  getByNumOfBuyers(buyers: number): Observable<GiftNewDTO[]> {
    return this.http.get<GiftNewDTO[]>(`${this.BASE_URL}/sort/buyers/${buyers}`);
  }

  // sortByPrice(): Observable<Gift[]> {
  //   return this.http.get<Gift[]>(`${this.BASE_URL}/sort/gift_price`);
  // }

  sortByAmountPeople(): Observable<Gift[]> {
    return this.http.get<Gift[]>(`${this.BASE_URL}/sort/amount_buyers`);
  }
  
  getWinners(): Observable<{ gift: string, winnerName: string, giftId: number, winnerUserId: number }[]> {
    return this.http.get<{ gift: string, winnerName: string, giftId: number, winnerUserId: number }[]>(`${this.BASE_URL}/winners`);
  }
  
  uploadImage(file: File): Observable<{ fileName: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ fileName: string }>(`${this.BASE_URL}/upload`, formData);
  }

  getParticipantsCount(giftId: number): Observable<number> {
  return this.http.get<any[]>(`${this.BASE_URL}/${giftId}/participants`)
    .pipe(
      map(data => {
        return data ? data.length : 0;
      }),
      catchError(() => of(0)) 
    );
}
}
