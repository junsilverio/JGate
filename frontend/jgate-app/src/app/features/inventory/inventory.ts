import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Product, Category, Supplier, StockLevel } from '../../shared/models';

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div>
      <h1 class="page-title mb-1">Inventory</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb"><li class="breadcrumb-item active">Inventory Management</li></ol>
      </nav>
      <hr />

      <!-- Tabs -->
      <ul class="nav nav-tabs mb-4">
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='products'" (click)="activeTab.set('products')">Products</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='stock'" (click)="activeTab.set('stock')">Stock Levels</a></li>
        <li class="nav-item"><a class="nav-link" [class.active]="activeTab()==='suppliers'" (click)="activeTab.set('suppliers')">Suppliers</a></li>
      </ul>

      <!-- Products Tab -->
      @if (activeTab() === 'products') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between align-items-center">
            <span class="fw-semibold">Products ({{ products().length }})</span>
            <button class="btn btn-primary btn-sm" (click)="showProductForm.set(true)">
              <i class="bi bi-plus-lg me-1"></i> Add Product
            </button>
          </div>

          @if (showProductForm()) {
            <div class="card-body border-bottom bg-light">
              <h6 class="fw-semibold mb-3">New Product</h6>
              <div class="row g-2">
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Product name *" [(ngModel)]="newProduct.name" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="SKU" [(ngModel)]="newProduct.sku" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" type="number" placeholder="Unit price" [(ngModel)]="newProduct.unitPrice" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Unit (kg, pcs)" [(ngModel)]="newProduct.unit" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" type="number" placeholder="Min stock" [(ngModel)]="newProduct.minStockLevel" /></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createProduct()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showProductForm.set(false)">Cancel</button>
              </div>
            </div>
          }

          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Name</th><th>SKU</th><th>Category</th><th>Supplier</th><th>Unit Price</th><th>Unit</th><th>Min Stock</th><th>Status</th><th>Cold</th></tr>
              </thead>
              <tbody>
                @for (p of products(); track p.id) {
                  <tr>
                    <td class="fw-semibold">{{ p.name }}</td>
                    <td class="text-muted small">{{ p.sku ?? '—' }}</td>
                    <td><span class="badge bg-secondary">{{ p.categoryName }}</span></td>
                    <td class="small">{{ p.supplierName ?? '—' }}</td>
                    <td class="fw-semibold">{{ p.unitPrice | number:'1.2-2' }}</td>
                    <td>{{ p.unit }}</td>
                    <td>{{ p.minStockLevel }}</td>
                    <td><span class="badge" [class]="p.isActive ? 'bg-success' : 'bg-secondary'">{{ p.isActive ? 'Active' : 'Inactive' }}</span></td>
                    <td>@if (p.requiresColdStorage) { <i class="bi bi-thermometer-low text-info"></i> }</td>
                  </tr>
                }
                @if (products().length === 0) {
                  <tr><td colspan="9" class="text-center text-muted py-4">No products found. Add your first product.</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Stock Levels Tab -->
      @if (activeTab() === 'stock') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 fw-semibold">
            Stock Levels
            @if (lowStockItems().length > 0) {
              <span class="badge bg-danger ms-2">{{ lowStockItems().length }} Low Stock</span>
            }
          </div>
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Product</th><th>Warehouse</th><th>On Hand</th><th>Reserved</th><th>Available</th><th>Alert</th></tr>
              </thead>
              <tbody>
                @for (s of stockLevels(); track s.id) {
                  <tr [class.table-danger]="s.quantityAvailable < 10">
                    <td class="fw-semibold">{{ s.productName }}</td>
                    <td>{{ s.warehouseName }}</td>
                    <td>{{ s.quantityOnHand }}</td>
                    <td>{{ s.quantityReserved }}</td>
                    <td class="fw-bold" [class.text-danger]="s.quantityAvailable < 10" [class.text-success]="s.quantityAvailable >= 10">{{ s.quantityAvailable }}</td>
                    <td>@if (s.quantityAvailable < 10) { <span class="badge bg-danger">Low Stock</span> }</td>
                  </tr>
                }
                @if (stockLevels().length === 0) {
                  <tr><td colspan="6" class="text-center text-muted py-4">No stock levels recorded.</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }

      <!-- Suppliers Tab -->
      @if (activeTab() === 'suppliers') {
        <div class="card border-0 shadow-sm">
          <div class="card-header bg-white border-0 d-flex justify-content-between align-items-center">
            <span class="fw-semibold">Suppliers ({{ suppliers().length }})</span>
            <button class="btn btn-primary btn-sm" (click)="showSupplierForm.set(true)">
              <i class="bi bi-plus-lg me-1"></i> Add Supplier
            </button>
          </div>

          @if (showSupplierForm()) {
            <div class="card-body border-bottom bg-light">
              <h6 class="fw-semibold mb-3">New Supplier</h6>
              <div class="row g-2">
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Company name *" [(ngModel)]="newSupplier.name" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Contact person" [(ngModel)]="newSupplier.contactPerson" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Email" [(ngModel)]="newSupplier.email" /></div>
                <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Phone" [(ngModel)]="newSupplier.phone" /></div>
                <div class="col-md-3"><input class="form-control form-control-sm" placeholder="Address" [(ngModel)]="newSupplier.address" /></div>
              </div>
              <div class="mt-2 d-flex gap-2">
                <button class="btn btn-sm btn-primary" (click)="createSupplier()">Save</button>
                <button class="btn btn-sm btn-outline-secondary" (click)="showSupplierForm.set(false)">Cancel</button>
              </div>
            </div>
          }

          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Name</th><th>Contact</th><th>Email</th><th>Phone</th><th>Status</th></tr>
              </thead>
              <tbody>
                @for (s of suppliers(); track s.id) {
                  <tr>
                    <td class="fw-semibold">{{ s.name }}</td>
                    <td>{{ s.contactPerson ?? '—' }}</td>
                    <td>{{ s.email ?? '—' }}</td>
                    <td>{{ s.phone ?? '—' }}</td>
                    <td><span class="badge" [class]="s.isActive ? 'bg-success' : 'bg-secondary'">{{ s.isActive ? 'Active' : 'Inactive' }}</span></td>
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
export class InventoryComponent implements OnInit {
  activeTab = signal('products');
  products = signal<Product[]>([]);
  categories = signal<Category[]>([]);
  suppliers = signal<Supplier[]>([]);
  stockLevels = signal<StockLevel[]>([]);
  lowStockItems = signal<StockLevel[]>([]);
  showProductForm = signal(false);
  showSupplierForm = signal(false);

  newProduct: any = { name: '', sku: '', unitPrice: 0, unit: 'kg', minStockLevel: 0 };
  newSupplier: any = { name: '', contactPerson: '', email: '', phone: '', address: '' };

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    const api = environment.apiUrl;
    this.http.get<Product[]>(`${api}/api/products`).subscribe({ next: v => this.products.set(v), error: () => {} });
    this.http.get<Supplier[]>(`${api}/api/suppliers`).subscribe({ next: v => this.suppliers.set(v), error: () => {} });
    this.http.get<StockLevel[]>(`${api}/api/stock`).subscribe({ next: v => this.stockLevels.set(v), error: () => {} });
    this.http.get<StockLevel[]>(`${api}/api/stock/low`).subscribe({ next: v => this.lowStockItems.set(v), error: () => {} });
  }

  createProduct() {
    const api = environment.apiUrl;
    const body = { ...this.newProduct, categoryId: '00000000-0000-0000-0000-000000000000', requiresColdStorage: false };
    this.http.post(`${api}/api/products`, body).subscribe({ next: () => { this.showProductForm.set(false); this.loadAll(); }, error: () => {} });
  }

  createSupplier() {
    const api = environment.apiUrl;
    this.http.post(`${api}/api/suppliers`, this.newSupplier).subscribe({ next: () => { this.showSupplierForm.set(false); this.loadAll(); }, error: () => {} });
  }
}
