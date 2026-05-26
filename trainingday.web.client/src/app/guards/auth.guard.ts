import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { AuthService } from '../services/auth/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> | boolean {
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login']);
      return false;
    }

    return this.authService.checkAuth().pipe(
      tap((res: any) => this.authService.updateUserFromServer(res)),
      map(() => true),
      catchError(() => {
        this.authService.clearUser();
        this.router.navigate(['/login']);
        return of(false);
      })
    );
  }
}
