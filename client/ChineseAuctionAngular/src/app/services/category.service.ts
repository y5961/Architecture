
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateGiftCategoryDTO, GiftCategoryDTO, UpdateGiftCategoryDTO } from '../models/CategoryDTO';
@Injectable({
  providedIn: 'root'
})
export class GiftCategoryService {
  readonly BASE_URL="api/GiftCategory";

  constructor(private http: HttpClient) {}

  getAllCategories(): Observable<GiftCategoryDTO[]> {
    return this.http.get<GiftCategoryDTO[]>(this.BASE_URL);
  }

  getCategoryById(id: number): Observable<GiftCategoryDTO> {
    return this.http.get<GiftCategoryDTO>(`${this.BASE_URL}/${id}`);
  }

  createCategory(dto: CreateGiftCategoryDTO): Observable<GiftCategoryDTO> {
    return this.http.post<GiftCategoryDTO>(this.BASE_URL, dto);
  }

  updateCategory(id: number, dto: UpdateGiftCategoryDTO): Observable<boolean> {
    return this.http.put<boolean>(`${this.BASE_URL}/${id}`, dto);
  }

  deleteCategory(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.BASE_URL}/${id}`);
  }
}

export { GiftCategoryService as CategoryService };