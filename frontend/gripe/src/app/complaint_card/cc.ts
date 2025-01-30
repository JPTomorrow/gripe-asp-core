import { Component, Input } from '@angular/core';
import { ApiService, Complaint, User } from '../api.service';
import { TablerIconComponent, provideTablerIcons } from "angular-tabler-icons";
import {
  IconThumbUp,
  IconThumbDown,
} from "angular-tabler-icons/icons"

@Component({
  selector: 'complaint-card',
  imports: [TablerIconComponent],
  providers: [provideTablerIcons({
    IconThumbUp,
    IconThumbDown,
  })],
  templateUrl: './cc.html',
})
  
export class ComplaintCard {

  @Input() complaint?: Complaint
  
  date: string | undefined
  username: string | undefined
  voted: boolean = false

  constructor(private apiService: ApiService) {}
 
  ngOnInit(): void {
    this.date = new Date(this.complaint!.submittedOn).toLocaleString()

    // get username
    this.apiService.getData<User>(`/users/${this.complaint?.userId}`).subscribe({
      next: (response) => this.username = response.username,
      error: (err) => console.error(err.message),
    });
  }

  handleThumbsUp() {
    this.apiService.incrementThumbsUp(this.complaint!.id).subscribe({
      next: (_) => {
        this.complaint!.thumbsUp++;
        this.voted = true
      },
      error: (err) => console.error(err.message),
    });
  }

  handleThumbsDown() {
    this.apiService.incrementThumbDown(this.complaint!.id).subscribe({
      next: (_) => {
        this.complaint!.thumbsDown++;
        this.voted = true;
      },
      error: (err) => console.error(err.message),
    });
  }
}

