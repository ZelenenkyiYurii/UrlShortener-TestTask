import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UrlsPageDto, UrlListItemDto } from './models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UrlApiService {
  constructor(private http: HttpClient) {}

  getAll(): Observable<UrlsPageDto> {
    return this.http.get<UrlsPageDto>('/api/urls', { withCredentials: true });
  }

  create(originalUrl: string): Observable<UrlListItemDto> {
    return this.http.post<UrlListItemDto>('/api/urls', { originalUrl }, { withCredentials: true });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`/api/urls/${id}`, { withCredentials: true });
  }
}
