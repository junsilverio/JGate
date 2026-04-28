import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../core/auth.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="app-wrapper">
      <!-- Sidebar -->
      <nav class="sidebar">
        <div class="sidebar-brand">
          <span>🥩</span>
          <span>JGate</span>
        </div>
        <div class="pt-2">
          <div class="nav-group-title">Main</div>
          <a routerLink="/dashboard" routerLinkActive="active" class="nav-link">
            <i class="bi bi-speedometer2"></i> Dashboard
          </a>
          <div class="nav-group-title">Operations</div>
          <a routerLink="/inventory" routerLinkActive="active" class="nav-link">
            <i class="bi bi-box-seam"></i> Inventory
          </a>
          <a routerLink="/orders" routerLinkActive="active" class="nav-link">
            <i class="bi bi-cart3"></i> Orders
          </a>
          <a routerLink="/delivery" routerLinkActive="active" class="nav-link">
            <i class="bi bi-truck"></i> Delivery
          </a>
          <a routerLink="/tasks" routerLinkActive="active" class="nav-link">
            <i class="bi bi-check2-square"></i> Tasks
          </a>
        </div>
      </nav>

      <!-- Main Content -->
      <div class="main-content">
        <!-- Topbar -->
        <div class="topbar">
          <div>
            <span class="fw-semibold text-muted small">Meat Manufacturing Management System</span>
          </div>
          <div class="d-flex align-items-center gap-3">
            <span class="badge bg-primary">{{ auth.userRole() }}</span>
            <span class="text-sm">{{ auth.user()?.fullName }}</span>
            <button class="btn btn-sm btn-outline-secondary" (click)="auth.logout()">
              <i class="bi bi-box-arrow-right"></i> Logout
            </button>
          </div>
        </div>
        <!-- Page Content -->
        <div class="page-content">
          <router-outlet></router-outlet>
        </div>
      </div>
    </div>
  `
})
export class ShellComponent {
  constructor(public auth: AuthService) {}
}
