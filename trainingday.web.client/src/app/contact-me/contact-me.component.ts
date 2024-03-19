import { Component, ViewChild, ElementRef } from '@angular/core';
import { BackendService } from '../services/backend/backend.service';

@Component({
  selector: 'app-contact-me',
  templateUrl: './contact-me.component.html',
  styleUrls: ['./contact-me.component.css']
})
export class ContactMeComponent {
  isShowBadge = false;

  name:string = "";
  email:string = "";
  message:string = "";
  constructor(private backendService: BackendService) {}

  sendMessage() : void {
    this.backendService.sendMessage(this.name, this.email, this.message).subscribe(() => {
      this.showBadge();
    }, error => {
      console.error('Failed to send message', error);
    });
  }

  showBadge() {
    this.isShowBadge = true;

    setTimeout(() => {
      this.isShowBadge = true;
    }, 2000);
  }
}
