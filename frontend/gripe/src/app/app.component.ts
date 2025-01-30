import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ApiService, Complaint } from './api.service';

import { ComplaintCard } from './complaint_card/cc'
import { FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button'; 



@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet, ComplaintCard, ReactiveFormsModule,
    MatFormFieldModule, MatSelectModule, MatButtonModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
  
export class AppComponent implements OnInit {
  title = 'gripe'
  
  companyNames: string[] | undefined 
  selectedCompany = new FormControl('', Validators.required); // Validation example
  complaints: Complaint[]
  constructor(private apiService: ApiService) {
    this.complaints = []
   }
  
  ngOnInit(): void {

    this.apiService.getData<string[]>("/complaints/company").subscribe({
      next: (response) => this.companyNames = response,
      error: (err) => console.error(err.message),
    });
  }

  handleSelectionChange(company: any) {
    console.log('Selected company:', company);
    this.apiService.getData<Complaint[]>(`/complaints/company/${company.value}`).subscribe({
      next: (response) => this.complaints = response,
      error: (err) => console.error(err.message),
    });
  }


  isShowingReviewForm: boolean = false
  showReviewForm(_: any) {
    this.isShowingReviewForm = true
  }

}

