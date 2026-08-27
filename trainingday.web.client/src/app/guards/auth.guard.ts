import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of, tap } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn) {
    return router.createUrlTree(['/login']);
  }

  return authService.checkAuth().pipe(
    tap((res: any) => authService.updateUserFromServer(res)),
    map(() => true),
    catchError(() => {
      authService.clearUser();
      return of(router.createUrlTree(['/login']));
    })
  );
};
