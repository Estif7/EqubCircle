import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CircleDetails, CircleMember, CircleSummary, CreateCircleRequest } from '../models/circle.model';

@Injectable({
  providedIn: 'root'
})
export class CircleService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/circles`;

  getAvailableCircles(): Observable<CircleSummary[]> {
    return this.http.get<CircleSummary[]>(this.apiUrl);
  }

  getMyCircles(): Observable<CircleSummary[]> {
    return this.http.get<CircleSummary[]>(`${this.apiUrl}/my`);
  }

  getCircleDetails(id: string): Observable<CircleDetails> {
    return this.http.get<CircleDetails>(`${this.apiUrl}/${id}`);
  }

  createCircle(request: CreateCircleRequest): Observable<CircleDetails> {
    return this.http.post<CircleDetails>(this.apiUrl, request);
  }

  joinCircle(id: string): Observable<CircleMember> {
    return this.http.post<CircleMember>(`${this.apiUrl}/${id}/join`, {});
  }

  startCircle(id: string): Observable<CircleDetails> {
    return this.http.post<CircleDetails>(`${this.apiUrl}/${id}/start`, {});
  }

  advanceRound(id: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${id}/advance-round`, {});
  }

  getCircleDashboard(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}/dashboard`);
  }
}
