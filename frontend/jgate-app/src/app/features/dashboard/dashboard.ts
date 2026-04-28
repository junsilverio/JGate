import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { environment } from '../../../environments/environment';
import { StockLevel, Order, DeliveryOrder, WorkTask } from '../../shared/models';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div>
      <h1 class="page-title mb-1">Dashboard</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb"><li class="breadcrumb-item active">Overview</li></ol>
      </nav>
      <hr />

      <!-- KPI Cards -->
      <div class="row g-3 mb-4">
        <div class="col-sm-6 col-xl-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex align-items-center justify-content-between">
                <div>
                  <div class="kpi-label">Low Stock Items</div>
                  <div class="kpi-value text-danger">{{ stats().lowStock }}</div>
                </div>
                <div class="kpi-icon bg-danger bg-opacity-10 text-danger">
                  <i class="bi bi-exclamation-triangle"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex align-items-center justify-content-between">
                <div>
                  <div class="kpi-label">Pending Orders</div>
                  <div class="kpi-value text-primary">{{ stats().pendingOrders }}</div>
                </div>
                <div class="kpi-icon bg-primary bg-opacity-10 text-primary">
                  <i class="bi bi-cart3"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex align-items-center justify-content-between">
                <div>
                  <div class="kpi-label">Active Deliveries</div>
                  <div class="kpi-value text-warning">{{ stats().activeDeliveries }}</div>
                </div>
                <div class="kpi-icon bg-warning bg-opacity-10 text-warning">
                  <i class="bi bi-truck"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-sm-6 col-xl-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex align-items-center justify-content-between">
                <div>
                  <div class="kpi-label">Open Tasks</div>
                  <div class="kpi-value text-success">{{ stats().openTasks }}</div>
                </div>
                <div class="kpi-icon bg-success bg-opacity-10 text-success">
                  <i class="bi bi-check2-square"></i>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Data -->
      <div class="row g-3">
        <!-- Recent Orders -->
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white border-0 fw-semibold d-flex justify-content-between">
              Recent Orders
              <a routerLink="/orders" class="btn btn-sm btn-outline-primary">View All</a>
            </div>
            <div class="card-body p-0">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Order #</th><th>Customer</th><th>Status</th><th>Total</th></tr>
                </thead>
                <tbody>
                  @for (order of recentOrders(); track order.id) {
                    <tr>
                      <td class="fw-semibold">{{ order.orderNumber }}</td>
                      <td>{{ order.customerName }}</td>
                      <td><span class="badge badge-{{ order.status }}">{{ order.status }}</span></td>
                      <td>{{ order.totalAmount | number:'1.2-2' }}</td>
                    </tr>
                  }
                  @if (recentOrders().length === 0) {
                    <tr><td colspan="4" class="text-center text-muted py-3">No orders yet</td></tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Recent Tasks -->
        <div class="col-lg-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white border-0 fw-semibold d-flex justify-content-between">
              Open Tasks
              <a routerLink="/tasks" class="btn btn-sm btn-outline-primary">View All</a>
            </div>
            <div class="card-body p-0">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr><th>Title</th><th>Priority</th><th>Status</th><th>Assignee</th></tr>
                </thead>
                <tbody>
                  @for (task of recentTasks(); track task.id) {
                    <tr>
                      <td>{{ task.title }}</td>
                      <td><span class="badge badge-{{ task.priority }}">{{ task.priority }}</span></td>
                      <td><span class="badge badge-{{ task.status }}">{{ task.status }}</span></td>
                      <td class="text-muted small">{{ task.assignedToUserName ?? '—' }}</td>
                    </tr>
                  }
                  @if (recentTasks().length === 0) {
                    <tr><td colspan="4" class="text-center text-muted py-3">No open tasks</td></tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  stats = signal({ lowStock: 0, pendingOrders: 0, activeDeliveries: 0, openTasks: 0 });
  recentOrders = signal<Order[]>([]);
  recentTasks = signal<WorkTask[]>([]);

  constructor(private http: HttpClient) {}

  ngOnInit() {
    const api = environment.apiUrl;
    forkJoin({
      lowStock: this.http.get<StockLevel[]>(`${api}/api/stock/low`),
      orders: this.http.get<Order[]>(`${api}/api/orders`),
      deliveries: this.http.get<DeliveryOrder[]>(`${api}/api/deliveries`),
      tasks: this.http.get<WorkTask[]>(`${api}/api/tasks`)
    }).subscribe({
      next: (data) => {
        this.stats.set({
          lowStock: data.lowStock.length,
          pendingOrders: data.orders.filter(o => o.status === 'Confirmed' || o.status === 'Draft').length,
          activeDeliveries: data.deliveries.filter(d => d.status === 'InTransit').length,
          openTasks: data.tasks.filter(t => t.status === 'Open' || t.status === 'InProgress').length
        });
        this.recentOrders.set(data.orders.slice(0, 5));
        this.recentTasks.set(data.tasks.filter(t => t.status !== 'Done').slice(0, 5));
      },
      error: () => {} // Silently fail if API not available
    });
  }
}
