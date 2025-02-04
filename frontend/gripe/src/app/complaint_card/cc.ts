import { Component, Input } from '@angular/core';
import { ApiService, Complaint, User } from '../api.service';
import { TablerIconComponent, provideTablerIcons } from "angular-tabler-icons";
import {
  IconThumbUp,
  IconThumbDown,
} from "angular-tabler-icons/icons"
import { animate, state, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'complaint-card',
  imports: [TablerIconComponent],
  providers: [provideTablerIcons({
    IconThumbUp,
    IconThumbDown,
  })],
  templateUrl: './cc.html',
  animations: [
    trigger("fade-in", [
      state(
        'hidden',
        style({
          opacity: 0.0,
          transform: 'translateY(50px)'
        }),
        
    ),
    state(
      'show',
      style({
        opacity: 1.0,
        transform: 'translateX(0px)'
      }),
      
    ),
      transition("hidden => show", animate(200)),
      transition("show => hidden", animate(200)),
    ]
    )],
})
  
export class ComplaintCard {

  @Input() complaint?: Complaint
  
  date: string | undefined
  username: string | undefined
  voted: boolean = false
  isShowing = false

  constructor(private apiService: ApiService) {}
 
  ngOnInit(): void {
    this.date = new Date(this.complaint!.submittedOn).toLocaleDateString()

    // get username
    this.apiService.getData<User>(`/users/${this.complaint?.userId}`).subscribe({
      next: (response) => this.username = response.username,
      error: (err) => console.error(err.message),
    });
    setTimeout(() => this.isShowing = true) // fade in
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

