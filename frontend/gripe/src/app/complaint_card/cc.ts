import { Component, Input } from '@angular/core';
import { ApiService, Complaint } from '../api.service';

@Component({
  selector: 'complaint-card',
  templateUrl: './cc.html',
})
  
export class ComplaintCard {

  @Input() complaint?: Complaint
  
  date: string | undefined
  username: string | undefined

  constructor(private apiService: ApiService) {}
 
  ngOnInit(): void {
    this.date = new Date(this.complaint!.submittedOn).toLocaleString()

    // get username
    this.apiService.getData<string>(`/users/get-name/${this.complaint?.userId}`).subscribe({
      next: (response) => this.username = response,
      error: (err) => console.error(err.message),
    });
  }
}

