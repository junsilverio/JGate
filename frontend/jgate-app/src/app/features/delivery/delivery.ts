import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Driver, Vehicle, DeliveryRoute, DeliveryOrder } from '../../shared/models';

@Component({
  selector: 'app-delivery',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div>
      <h1 class="page-title mb-1">Delivery</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb"><li class="breadcrumb-item active">Delivery Management</li></ol>
      </nav>
      <hr />

      <ul class="nav nav-tabs mb-4">
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='deliveries'" (click)="activeTab.set('deliveries')">Deliveries</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='routes'" (click)="activeTab.set('routes')">Routes</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='drivers'" (click)="activeTab.set('drivers')">Drivers</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='vehicles'" (click)="activeTab.set('vehicles')">Vehicles</a></li>
      </ul>

      <!-- Deliveries Tab -->
      @if (activeTab() === 'deliveries') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 fw-semibold">
            All Deliveries
            <div class="float-end d-flex gap-2">
              @for (status of deliveryStatuses; track status) {
                <span class="badge badge-{{ status }}">
                  {{ deliveries().filter(d => d.status === status).length }} {{ status }}
                </span>
              }
            </div>
          </div>
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Order #</th><th>Status</th><th>Delivered At</th><th>Actions</th></tr>
              </thead>
              <tbody>
                @for (d of deliveries(); track d.id) {
                  <tr>
                    <td class="fw-semibold">{{ d.orderNumber }}</td>
                    <td><span class="badge badge-{{ d.status }}">{{ d.status }}</span></td>
                    <td class="small text-muted">{{ (d.deliveredAt | date:'medium') ?? '—' }}</td>
                    <td>
                      @if (d.status === 'Assigned') {
                        <button class="btn btn-xs btn-outline-primary" (click)="updateDeliveryStatus(d.id, 'InTransit')">Start Transit</button>
                      }
                      @if (d.status === 'InTransit') {
                        <button class="btn btn-xs btn-outline-success me-1" (click)="updateDeliveryStatus(d.id, 'Delivered')">Delivered</button>
                        <button class="btn btn-xs btn-outline-danger" (click)="updateDeliveryStatus(d.id, 'Failed')">Failed</button>
                      }
                    </td>
                  </tr>
                }
                @if (deliveries().length === 0) {
                  <tr><td colspan="4" class="text-center text-muted py-4">No deliveries found.</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Routes Tab -->
      @if (activeTab() === 'routes') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between">
            <span class="fw-semibold">Delivery Routes</span>
            <button class="btn btn-primary btn-sm" (click)="showRouteForm.set(true)"><i class="bi bi-plus-lg me-1"></i>New Route</button>
          </div>
          @if (showRouteForm()) {
            <div class="card-body border-bottom bg-light">
              <div class="row g-2">
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Route name *" [(ngModel)]="newRoute.name" /></div>
                <div class="col-md-3"><input class="form-control form-control-sm" type="datetime-local" [(ngModel)]="newRoute.plannedDate" /></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createRoute()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showRouteForm.set(false)">Cancel</button>
              </div>
            </div>
          }
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Code</th><th>Name</th><th>Date</th><th>Driver</th><th>Vehicle</th></tr>
              </thead>
              <tbody>
                @for (r of routes(); track r.id) {
                  <tr>
                    <td class="fw-semibold text-primary">{{ r.routeCode }}</td>
                    <td>{{ r.name }}</td>
                    <td class="small">{{ r.plannedDate | date:'mediumDate' }}</td>
                    <td>{{ r.driverName ?? '—' }}</td>
                    <td>{{ r.vehiclePlate ?? '—' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Drivers Tab -->
      @if (activeTab() === 'drivers') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between">
            <span class="fw-semibold">Drivers</span>
            <button class="btn btn-primary btn-sm" (click)="showDriverForm.set(true)"><i class="bi bi-plus-lg me-1"></i>Add Driver</button>
          </div>
          @if (showDriverForm()) {
            <div class="card-body border-bottom bg-light">
              <div class="row g-2">
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="First name *" [(ngModel)]="newDriver.firstName" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Last name *" [(ngModel)]="newDriver.lastName" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Email" [(ngModel)]="newDriver.email" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Phone" [(ngModel)]="newDriver.phone" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="License #" [(ngModel)]="newDriver.licenseNumber" /></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createDriver()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showDriverForm.set(false)">Cancel</button>
              </div>
            </div>
          }
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Name</th><th>Email</th><th>Phone</th><th>License</th><th>Available</th></tr>
              </thead>
              <tbody>
                @for (d of drivers(); track d.id) {
                  <tr>
                    <td class="fw-semibold">{{ d.firstName }} {{ d.lastName }}</td>
                    <td>{{ d.email ?? '—' }}</td>
                    <td>{{ d.phone ?? '—' }}</td>
                    <td>{{ d.licenseNumber ?? '—' }}</td>
                    <td><span class="badge" [class]="d.isAvailable ? 'bg-success' : 'bg-warning text-dark'">{{ d.isAvailable ? 'Available' : 'Busy' }}</span></td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Vehicles Tab -->
      @if (activeTab() === 'vehicles') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between">
            <span class="fw-semibold">Fleet</span>
            <button class="btn btn-primary btn-sm" (click)="showVehicleForm.set(true)"><i class="bi bi-plus-lg me-1"></i>Add Vehicle</button>
          </div>
          @if (showVehicleForm()) {
            <div class="card-body border-bottom bg-light">
              <div class="row g-2">
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Plate # *" [(ngModel)]="newVehicle.plateNumber" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Make" [(ngModel)]="newVehicle.make" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Model" [(ngModel)]="newVehicle.model" /></div>
                <div class="col-md-1"><input class="form-control form-control-sm" placeholder="Year" type="number" [(ngModel)]="newVehicle.year" /></div>
                <div class="col-md-2">
                  <select class="form-select form-select-sm" [(ngModel)]="newVehicle.type">
                    <option>RefrigeratedTruck</option><option>Van</option><option>Motorcycle</option><option>Other</option>
                  </select>
                </div>
                <div class="col-md-1"><div class="form-check mt-1"><input class="form-check-input" type="checkbox" [(ngModel)]="newVehicle.hasTemperatureControl" /><label class="form-check-label small">Temp</label></div></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createVehicle()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showVehicleForm.set(false)">Cancel</button>
              </div>
            </div>
          }
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Plate</th><th>Make/Model</th><th>Year</th><th>Type</th><th>Max Load</th><th>Temp Control</th><th>Status</th></tr>
              </thead>
              <tbody>
                @for (v of vehicles(); track v.id) {
                  <tr>
                    <td class="fw-semibold">{{ v.plateNumber }}</td>
                    <td>{{ v.make }} {{ v.model }}</td>
                    <td>{{ v.year }}</td>
                    <td><span class="badge bg-info text-dark">{{ v.type }}</span></td>
                    <td>{{ v.maxLoadKg ?? '—' }} kg</td>
                    <td>@if (v.hasTemperatureControl) { <i class="bi bi-thermometer text-info"></i> } @else { <span class="text-muted">—</span> }</td>
                    <td><span class="badge" [class]="v.isAvailable ? 'bg-success' : 'bg-warning text-dark'">{{ v.isAvailable ? 'Available' : 'In Use' }}</span></td>
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
export class DeliveryComponent implements OnInit {
  activeTab = signal('deliveries');
  drivers = signal<Driver[]>([]);
  vehicles = signal<Vehicle[]>([]);
  routes = signal<DeliveryRoute[]>([]);
  deliveries = signal<DeliveryOrder[]>([]);
  showDriverForm = signal(false);
  showVehicleForm = signal(false);
  showRouteForm = signal(false);
  newDriver: any = { firstName: '', lastName: '', email: '', phone: '', licenseNumber: '' };
  newVehicle: any = { plateNumber: '', make: '', model: '', year: new Date().getFullYear(), type: 'RefrigeratedTruck', hasTemperatureControl: true };
  newRoute: any = { name: '', plannedDate: '' };

  readonly deliveryStatuses = ['Pending', 'Assigned', 'InTransit', 'Delivered', 'Failed'];

  constructor(private http: HttpClient) {}

  ngOnInit() { this.loadAll(); }

  loadAll() {
    const api = environment.apiUrl;
    this.http.get<Driver[]>(`${api}/api/drivers`).subscribe({ next: v => this.drivers.set(v), error: () => {} });
    this.http.get<Vehicle[]>(`${api}/api/vehicles`).subscribe({ next: v => this.vehicles.set(v), error: () => {} });
    this.http.get<DeliveryRoute[]>(`${api}/api/routes`).subscribe({ next: v => this.routes.set(v), error: () => {} });
    this.http.get<DeliveryOrder[]>(`${api}/api/deliveries`).subscribe({ next: v => this.deliveries.set(v), error: () => {} });
  }

  updateDeliveryStatus(id: string, status: string) {
    this.http.patch(`${environment.apiUrl}/api/deliveries/${id}/status`, { status }).subscribe({ next: () => this.loadAll(), error: () => {} });
  }
  createDriver() {
    this.http.post(`${environment.apiUrl}/api/drivers`, this.newDriver).subscribe({ next: () => { this.showDriverForm.set(false); this.loadAll(); }, error: () => {} });
  }
  createVehicle() {
    this.http.post(`${environment.apiUrl}/api/vehicles`, this.newVehicle).subscribe({ next: () => { this.showVehicleForm.set(false); this.loadAll(); }, error: () => {} });
  }
  createRoute() {
    this.http.post(`${environment.apiUrl}/api/routes`, this.newRoute).subscribe({ next: () => { this.showRouteForm.set(false); this.loadAll(); }, error: () => {} });
  }
}
