import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { environment } from '../../../environment/environment';

export interface UserInfo {
  email: string;
  nick: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authApiUrl = `${environment.baseApiUrl}/auth`;
  private readonly storageKey = 'td_user';

  private _currentUser = new BehaviorSubject<UserInfo | null>(this.loadUser());
  currentUser$ = this._currentUser.asObservable();

  get isLoggedIn(): boolean {
    return this._currentUser.value !== null;
  }

  get isAdmin(): boolean {
    return this._currentUser.value?.role === 'Admin';
  }

  get currentUser(): UserInfo | null {
    return this._currentUser.value;
  }

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.authApiUrl}/login`, { email, password, rememberMe: true }, { withCredentials: true }).pipe(
      tap((res: any) => {
        this.setUser({ email: res.email, nick: res.nick ?? email.split('@')[0], role: res.role ?? 'User' });
      })
    );
  }

  register(email: string, password: string, nick: string): Observable<any> {
    return this.http.post(`${this.authApiUrl}/register`, { email, password, nick }, { withCredentials: true });
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.authApiUrl}/forgot`, { email }, { withCredentials: true });
  }

  confirmEmail(userId: string, code: string): Observable<any> {
    return this.http.get(`${this.authApiUrl}/confirm`, {
      params: { userId, code },
      withCredentials: true
    });
  }

  checkAuth(): Observable<any> {
    return this.http.get(`${this.authApiUrl}/enter`, { withCredentials: true });
  }

  logout(): Observable<any> {
    return this.http.post(`${this.authApiUrl}/logout`, {}, { withCredentials: true }).pipe(
      finalize(() => this.clearUser()),
    );
  }

  extractError(err: any): string {
    const body = err?.error;
    if (typeof body === 'string' && body.length > 0) return body;
    if (typeof body === 'object' && body !== null) {
      if (body.title) return body.title;
      const values = Object.values(body).flat();
      if (values.length) return (values as string[]).join(' ');
    }
    return 'An unexpected error occurred. Please try again.';
  }

  private setUser(user: UserInfo): void {
    localStorage.setItem(this.storageKey, JSON.stringify(user));
    this._currentUser.next(user);
  }

  clearUser(): void {
    localStorage.removeItem(this.storageKey);
    this._currentUser.next(null);
  }

  updateUserFromServer(res: any): void {
    if (res?.email) {
      this.setUser({ email: res.email, nick: res.nick ?? res.email.split('@')[0], role: res.role ?? 'User' });
    }
  }

  private loadUser(): UserInfo | null {
    try {
      const raw = localStorage.getItem(this.storageKey);
      if (!raw) return null;
      const user = JSON.parse(raw);
      return { ...user, role: user.role ?? 'User' };
    } catch {
      return null;
    }
  }
}
