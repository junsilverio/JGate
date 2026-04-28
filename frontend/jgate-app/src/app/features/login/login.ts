import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="login-page">
      <div class="login-card">
        <div class="text-center mb-4">
          <div class="login-logo">🥩 JGate</div>
          <div class="text-muted mt-1">Meat Manufacturing Management</div>
        </div>
        <form (ngSubmit)="login()">
          <div class="mb-3">
            <label class="form-label fw-semibold">Email</label>
            <input type="email" class="form-control form-control-lg" [(ngModel)]="email" name="email" required placeholder="admin@jgate.com" />
          </div>
          <div class="mb-3">
            <label class="form-label fw-semibold">Password</label>
            <input type="password" class="form-control form-control-lg" [(ngModel)]="password" name="password" required placeholder="••••••••" />
          </div>
          @if (error()) {
            <div class="alert alert-danger py-2">{{ error() }}</div>
          }
          <button type="submit" class="btn btn-primary w-100 btn-lg mt-2" [disabled]="loading()">
            @if (loading()) { <span class="spinner-border spinner-border-sm me-2"></span> }
            Sign In
          </button>
        </form>
      </div>
    </div>
  `
})
export class LoginComponent {
  email = '';
  password = '';
  loading = signal(false);
  error = signal('');

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.loading.set(true);
    this.error.set('');
    this.authService.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.message ?? 'Login failed. Please try again.');
      }
    });
  }
}
