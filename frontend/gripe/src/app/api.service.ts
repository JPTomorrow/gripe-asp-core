// api.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, ObservableLike, catchError } from 'rxjs';

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

export interface User {
  id: number,
  username: string,
  email: string,
  IpAddress: string,
  joinedDate: string
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

  incrementThumbsUp(complaintId: number): Observable<Object> {
    return this.http.post(`${this.apiUrl}/complaints/thumbs-up/${complaintId}`, {
      complaintId: complaintId
    }).pipe(
      catchError((error) => {
        console.error('Request failed:', error);
        throw new Error('Failed to fetch data');
      })
    );
  }

  incrementThumbDown(complaintId: number): Observable<Object> {
    return this.http.post(`${this.apiUrl}/complaints/thumbs-down/${complaintId}`, {
      complaintId: complaintId
    }).pipe(
      catchError((error) => {
        console.error('Request failed:', error);
        throw new Error('Failed to fetch data');
      })
    );
  }
}