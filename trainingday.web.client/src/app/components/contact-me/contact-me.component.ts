import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BackendService } from '../../services/backend/backend.service';

@Component({
  selector: 'app-contact-me',
  standalone: true,
  imports: [CommonModule, FormsModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
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
    this.backendService.sendMessage(this.name, this.email, this.message).subscribe({
      next: () => this.showBadge(),
      error: (error) => console.error('Failed to send message', error)
    });
  }

  showBadge() {
    this.isShowBadge = true;

    setTimeout(() => {
      this.isShowBadge = true;
    }, 2000);
  }
}
