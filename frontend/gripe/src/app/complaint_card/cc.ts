import { Component, Input} from '@angular/core';

@Component({
  selector: 'complaint-card',
  templateUrl: './cc.html',
})
  
export class ComplaintCard {
    @Input() complaint: any
    
}

