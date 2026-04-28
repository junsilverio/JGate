import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/login/login').then(m => m.LoginComponent) },
  {
    path: '',
    loadComponent: () => import('./layout/shell').then(m => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard').then(m => m.DashboardComponent) },
      { path: 'inventory', loadComponent: () => import('./features/inventory/inventory').then(m => m.InventoryComponent) },
      { path: 'orders', loadComponent: () => import('./features/orders/orders').then(m => m.OrdersComponent) },
      { path: 'delivery', loadComponent: () => import('./features/delivery/delivery').then(m => m.DeliveryComponent) },
      { path: 'tasks', loadComponent: () => import('./features/tasks/tasks').then(m => m.TasksComponent) },
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
