import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'jgate_token';
  private readonly userKey = 'jgate_user';

  private _user = signal<AuthResponse | null>(this.loadUser());
  user = this._user.asReadonly();
  isLoggedIn = computed(() => !!this._user());
  userRole = computed(() => this._user()?.role ?? '');

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(tap(res => this.saveAuth(res)));
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this._user.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private saveAuth(res: AuthResponse) {
    localStorage.setItem(this.tokenKey, res.accessToken);
    localStorage.setItem(this.userKey, JSON.stringify(res));
    this._user.set(res);
  }

  private loadUser(): AuthResponse | null {
    try {
      const stored = localStorage.getItem(this.userKey);
      return stored ? JSON.parse(stored) : null;
    } catch { return null; }
  }
}
