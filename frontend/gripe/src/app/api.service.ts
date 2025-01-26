// api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';

export interface Complaint {
  id: number
  userId: number
  companyName: string 
  title: string
  body: string 
  submittedOn: string
  thumbsUp: number
  thumbsDown: number
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private apiUrl = "http://localhost:5208"

  constructor(private http: HttpClient) { }

  // GET request with error handling
  getData<T>(endpointStr : string): Observable<T> {
    return this.http.get<T>(`${this.apiUrl}${endpointStr}`).pipe(
      catchError((error) => {
        console.error('Request failed:', error);
        throw new Error('Failed to fetch data');
      })
    );
  }
}