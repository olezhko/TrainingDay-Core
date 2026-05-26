import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {
  state: 'loading' | 'success' | 'error' = 'loading';
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const code = this.route.snapshot.queryParamMap.get('code');

    if (!userId || !code) {
      this.state = 'error';
      this.errorMessage = 'Invalid confirmation link. Please check your email and try again.';
      return;
    }

    this.authService.confirmEmail(userId, code).subscribe({
      next: () => { this.state = 'success'; },
      error: (err) => {
        this.state = 'error';
        this.errorMessage = typeof err?.error === 'string' ? err.error : 'Confirmation failed. The link may have expired.';
      }
    });
  }
}
