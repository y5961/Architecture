import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PackageDTO, PackageCreateDTO } from '../models/PackageDTO';
import { environment } from '../../../environment';

@Injectable({
  providedIn: 'root'
})
export class PackageService {
  private readonly BASE_URL = `${environment.apiUrl}/api/Packages`;
  private http = inject(HttpClient);

  getAllPackages(): Observable<PackageDTO[]> {
    return this.http.get<PackageDTO[]>(this.BASE_URL);
  }

  getPackageById(id: number): Observable<PackageDTO> {
    return this.http.get<PackageDTO>(`${this.BASE_URL}/${id}`);
  }

  createPackage(dto: PackageCreateDTO): Observable<number> {
    return this.http.post<number>(this.BASE_URL, dto);
  }

  updatePackage(id: number, dto: PackageCreateDTO): Observable<void> {
    return this.http.put<void>(`${this.BASE_URL}/${id}`, dto);
  }

  deletePackage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/${id}`);
  }
}