import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService } from './api.service';

import { ComplaintCard } from './complaint_card/cc'
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ComplaintCard],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
  
export class AppComponent implements OnInit {
  title = 'gripe';
  
  companyNames: string[] | undefined 
  constructor(private apiService: ApiService) { }
  
  ngOnInit(): void {

    this.apiService.getData<string[]>("/complaints/list-companies").subscribe({
      next: (response) => this.companyNames = response,
      error: (err) => console.error(err.message),
    });
  }

  items = [
    { id: 1, name: 'Item 1' },
    { id: 2, name: 'Item 2' },
  ];
  selectedItem = new FormControl('', Validators.required); // Validation example
}

