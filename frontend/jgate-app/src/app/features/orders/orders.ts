import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Order, Customer } from '../../shared/models';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div>
      <h1 class="page-title mb-1">Orders</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb"><li class="breadcrumb-item active">Order Management</li></ol>
      </nav>
      <hr />

      <!-- Summary Cards -->
      <div class="row g-3 mb-4">
        @for (stat of orderStats(); track stat.label) {
          <div class="col-sm-4 col-xl-2">
            <div class="card kpi-card text-center p-3">
              <div class="kpi-value" [class]="stat.colorClass">{{ stat.count }}</div>
              <div class="kpi-label">{{ stat.label }}</div>
            </div>
          </div>
        }
      </div>

      <!-- Tabs -->
      <ul class="nav nav-tabs mb-4">
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='orders'" (click)="activeTab.set('orders')">Orders</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='customers'" (click)="activeTab.set('customers')">Customers</a></li>
      </ul>

      <!-- Orders Tab -->
      @if (activeTab() === 'orders') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between align-items-center">
            <span class="fw-semibold">Orders ({{ orders().length }})</span>
            <div class="d-flex gap-2">
              <select class="form-select form-select-sm" [(ngModel)]="statusFilter" (change)="filterOrders()">
                <option value="">All Statuses</option>
                <option>Draft</option><option>Confirmed</option><option>Processing</option>
                <option>Ready</option><option>Delivered</option><option>Cancelled</option>
              </select>
            </div>
          </div>
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Order #</th><th>Customer</th><th>Date</th><th>Required</th><th>Status</th><th>Total</th><th>Actions</th></tr>
              </thead>
              <tbody>
                @for (o of filteredOrders(); track o.id) {
                  <tr>
                    <td class="fw-semibold text-primary">{{ o.orderNumber }}</td>
                    <td>{{ o.customerName }}</td>
                    <td class="small text-muted">{{ o.orderDate | date:'mediumDate' }}</td>
                    <td class="small text-muted">{{ (o.requiredDate | date:'mediumDate') ?? '—' }}</td>
                    <td><span class="badge badge-{{ o.status }}">{{ o.status }}</span></td>
                    <td class="fw-semibold">{{ o.totalAmount | number:'1.2-2' }}</td>
                    <td>
                      <div class="d-flex gap-1">
                        @if (o.status === 'Draft') {
                          <button class="btn btn-xs btn-outline-primary" (click)="updateStatus(o.id, 'Confirmed')">Confirm</button>
                        }
                        @if (o.status === 'Confirmed') {
                          <button class="btn btn-xs btn-outline-warning" (click)="updateStatus(o.id, 'Processing')">Process</button>
                        }
                        @if (o.status === 'Processing') {
                          <button class="btn btn-xs btn-outline-success" (click)="updateStatus(o.id, 'Ready')">Ready</button>
                        }
                      </div>
                    </td>
                  </tr>
                }
                @if (filteredOrders().length === 0) {
                  <tr><td colspan="7" class="text-center text-muted py-4">No orders found.</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Customers Tab -->
      @if (activeTab() === 'customers') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between align-items-center">
            <span class="fw-semibold">Customers ({{ customers().length }})</span>
            <button class="btn btn-primary btn-sm" (click)="showCustomerForm.set(true)">
              <i class="bi bi-plus-lg me-1"></i> Add Customer
            </button>
          </div>

          @if (showCustomerForm()) {
            <div class="card-body border-bottom bg-light">
              <div class="row g-2">
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Company name *" [(ngModel)]="newCustomer.name" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Contact person" [(ngModel)]="newCustomer.contactPerson" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Email" [(ngModel)]="newCustomer.email" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Phone" [(ngModel)]="newCustomer.phone" /></div>
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Tax ID" [(ngModel)]="newCustomer.taxId" /></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createCustomer()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showCustomerForm.set(false)">Cancel</button>
              </div>
            </div>
          }

          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Name</th><th>Contact</th><th>Email</th><th>Phone</th><th>Tax ID</th><th>Status</th></tr>
              </thead>
              <tbody>
                @for (c of customers(); track c.id) {
                  <tr>
                    <td class="fw-semibold">{{ c.name }}</td>
                    <td>{{ c.contactPerson ?? '—' }}</td>
                    <td>{{ c.email ?? '—' }}</td>
                    <td>{{ c.phone ?? '—' }}</td>
                    <td class="text-muted small">{{ c.taxId ?? '—' }}</td>
                    <td><span class="badge" [class]="c.isActive ? 'bg-success' : 'bg-secondary'">{{ c.isActive ? 'Active' : 'Inactive' }}</span></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }
    </div>
  `
})
export class OrdersComponent implements OnInit {
  activeTab = signal('orders');
  orders = signal<Order[]>([]);
  customers = signal<Customer[]>([]);
  filteredOrders = signal<Order[]>([]);
  statusFilter = '';
  showCustomerForm = signal(false);
  newCustomer: any = { name: '', contactPerson: '', email: '', phone: '', taxId: '' };

  orderStats = signal<{ label: string; count: number; colorClass: string }[]>([]);

  constructor(private http: HttpClient) {}

  ngOnInit() { this.loadAll(); }

  loadAll() {
    const api = environment.apiUrl;
    this.http.get<Order[]>(`${api}/api/orders`).subscribe({
      next: orders => {
        this.orders.set(orders);
        this.filteredOrders.set(orders);
        const statuses = ['Draft', 'Confirmed', 'Processing', 'Ready', 'Delivered', 'Cancelled'];
        const colors: Record<string, string> = { Draft: 'text-secondary', Confirmed: 'text-primary', Processing: 'text-warning', Ready: 'text-success', Delivered: 'text-info', Cancelled: 'text-danger' };
        this.orderStats.set(statuses.map(s => ({ label: s, count: orders.filter(o => o.status === s).length, colorClass: colors[s] })));
      }, error: () => {}
    });
    this.http.get<Customer[]>(`${api}/api/customers`).subscribe({ next: v => this.customers.set(v), error: () => {} });
  }

  filterOrders() {
    const orders = this.orders();
    this.filteredOrders.set(this.statusFilter ? orders.filter(o => o.status === this.statusFilter) : orders);
  }

  updateStatus(id: string, status: string) {
    const api = environment.apiUrl;
    this.http.patch(`${api}/api/orders/${id}/status`, { status }).subscribe({ next: () => this.loadAll(), error: () => {} });
  }

  createCustomer() {
    const api = environment.apiUrl;
    this.http.post(`${api}/api/customers`, this.newCustomer).subscribe({ next: () => { this.showCustomerForm.set(false); this.loadAll(); }, error: () => {} });
  }
}
