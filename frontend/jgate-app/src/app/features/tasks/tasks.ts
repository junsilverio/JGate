import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { WorkTask, TaskCategory } from '../../shared/models';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div>
      <h1 class="page-title mb-1">Tasks</h1>
      <nav aria-label="breadcrumb">
        <ol class="breadcrumb"><li class="breadcrumb-item active">Task Management</li></ol>
      </nav>
      <hr />

      <!-- Kanban Board -->
      <div class="d-flex justify-content-between align-items-center mb-3">
        <div class="d-flex gap-2">
          <button class="btn btn-sm" [class.btn-primary]="viewMode()==='kanban'" [class.btn-outline-secondary]="viewMode()!=='kanban'" (click)="viewMode.set('kanban')">
            <i class="bi bi-kanban me-1"></i>Kanban
          </button>
          <button class="btn btn-sm" [class.btn-primary]="viewMode()==='list'" [class.btn-outline-secondary]="viewMode()!=='list'" (click)="viewMode.set('list')">
            <i class="bi bi-list-ul me-1"></i>List
          </button>
        </div>
        <button class="btn btn-primary btn-sm" (click)="showTaskForm.set(true)">
          <i class="bi bi-plus-lg me-1"></i>New Task
        </button>
      </div>

      <!-- New Task Form -->
      @if (showTaskForm()) {
        <div class="card border-0 shadow-sm mb-3">
          <div class="card-body">
            <h6 class="fw-semibold mb-3">New Task</h6>
            <div class="row g-2">
              <div class="col-md-4"><input class="form-control form-control-sm" placeholder="Task title *" [(ngModel)]="newTask.title" /></div>
              <div class="col-md-2">
                <select class="form-select form-select-sm" [(ngModel)]="newTask.priority">
                  <option>Low</option><option>Medium</option><option>High</option><option>Critical</option>
                </select>
              </div>
              <div class="col-md-2"><input class="form-control form-control-sm" type="date" [(ngModel)]="newTask.dueDate" /></div>
              <div class="col-md-2"><input class="form-control form-control-sm" placeholder="Assigned to" [(ngModel)]="newTask.assignedToUserName" /></div>
              <div class="col-md-2"><textarea class="form-control form-control-sm" placeholder="Notes" [(ngModel)]="newTask.notes" rows="1"></textarea></div>
            </div>
            <div class="mt-2 d-flex gap-2">
              <button class="btn btn-sm btn-primary" (click)="createTask()">Create Task</button>
              <button class="btn btn-sm btn-outline-secondary" (click)="showTaskForm.set(false)">Cancel</button>
            </div>
          </div>
        </div>
      }

      <!-- Kanban View -->
      @if (viewMode() === 'kanban') {
        <div class="row g-3">
          @for (column of kanbanColumns; track column.status) {
            <div class="col-md-3">
              <div class="card border-0 shadow-sm h-100">
                <div class="card-header border-0 d-flex align-items-center justify-content-between" [style.background-color]="column.color + '22'">
                  <span class="fw-semibold" [style.color]="column.color">{{ column.label }}</span>
                  <span class="badge" [style.background-color]="column.color">{{ getTasksByStatus(column.status).length }}</span>
                </div>
                <div class="card-body p-2" style="min-height: 200px;">
                  @for (task of getTasksByStatus(column.status); track task.id) {
                    <div class="card mb-2 border-start border-3" [style.border-color]="getPriorityColor(task.priority) + ' !important'">
                      <div class="card-body p-2">
                        <div class="fw-semibold small">{{ task.title }}</div>
                        <div class="d-flex gap-1 mt-1 flex-wrap">
                          <span class="badge badge-{{ task.priority }} small">{{ task.priority }}</span>
                          <span class="badge bg-light text-dark small">{{ task.categoryName }}</span>
                        </div>
                        @if (task.assignedToUserName) {
                          <div class="text-muted" style="font-size:0.7rem;">👤 {{ task.assignedToUserName }}</div>
                        }
                        @if (task.dueDate) {
                          <div class="text-muted" style="font-size:0.7rem;">📅 {{ task.dueDate | date:'shortDate' }}</div>
                        }
                        <div class="d-flex gap-1 mt-2">
                          @if (column.status !== 'Done' && column.status !== 'Cancelled') {
                            <button class="btn btn-xs btn-outline-success" (click)="advanceTask(task)">→</button>
                          }
                        </div>
                      </div>
                    </div>
                  }
                </div>
              </div>
            </div>
          }
        </div>
      }

      <!-- List View -->
      @if (viewMode() === 'list') {
        <div class="card border-0 shadow-sm">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr><th>Title</th><th>Category</th><th>Priority</th><th>Status</th><th>Due Date</th><th>Assignee</th><th>Actions</th></tr>
              </thead>
              <tbody>
                @for (t of tasks(); track t.id) {
                  <tr>
                    <td class="fw-semibold">{{ t.title }}</td>
                    <td><span class="badge bg-secondary">{{ t.categoryName }}</span></td>
                    <td><span class="badge badge-{{ t.priority }}">{{ t.priority }}</span></td>
                    <td><span class="badge badge-{{ t.status }}">{{ t.status }}</span></td>
                    <td class="small text-muted">{{ (t.dueDate | date:'mediumDate') ?? '—' }}</td>
                    <td class="small">{{ t.assignedToUserName ?? '—' }}</td>
                    <td>
                      <select class="form-select form-select-sm" style="width:auto" (change)="updateTaskStatus(t.id, $any($event.target).value)">
                        <option value="">Move to...</option>
                        <option>Open</option><option>InProgress</option><option>Review</option><option>Done</option>
                      </select>
                    </td>
                  </tr>
                }
                @if (tasks().length === 0) {
                  <tr><td colspan="7" class="text-center text-muted py-4">No tasks yet. Create your first task!</td></tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      }
    </div>
  `
})
export class TasksComponent implements OnInit {
  tasks = signal<WorkTask[]>([]);
  categories = signal<TaskCategory[]>([]);
  viewMode = signal<'kanban' | 'list'>('kanban');
  showTaskForm = signal(false);

  kanbanColumns = [
    { status: 'Open', label: 'Open', color: '#6c757d' },
    { status: 'InProgress', label: 'In Progress', color: '#0d6efd' },
    { status: 'Review', label: 'Review', color: '#fd7e14' },
    { status: 'Done', label: 'Done', color: '#198754' }
  ];

  newTask: any = { title: '', priority: 'Medium', dueDate: '', assignedToUserName: '', notes: '' };

  constructor(private http: HttpClient) {}

  ngOnInit() { this.loadAll(); }

  loadAll() {
    const api = environment.apiUrl;
    this.http.get<WorkTask[]>(`${api}/api/tasks`).subscribe({ next: v => this.tasks.set(v), error: () => {} });
    this.http.get<TaskCategory[]>(`${api}/api/task-categories`).subscribe({ next: v => this.categories.set(v), error: () => {} });
  }

  getTasksByStatus(status: string): WorkTask[] {
    return this.tasks().filter(t => t.status === status);
  }

  getPriorityColor(priority: string): string {
    const map: Record<string, string> = { Low: '#20c997', Medium: '#fd7e14', High: '#dc3545', Critical: '#7209b7' };
    return map[priority] ?? '#6c757d';
  }

  advanceTask(task: WorkTask) {
    const flow: Record<string, string> = { Open: 'InProgress', InProgress: 'Review', Review: 'Done' };
    const next = flow[task.status];
    if (next) this.updateTaskStatus(task.id, next);
  }

  updateTaskStatus(id: string, status: string) {
    if (!status) return;
    this.http.patch(`${environment.apiUrl}/api/tasks/${id}/status`, { status }).subscribe({ next: () => this.loadAll(), error: () => {} });
  }

  createTask() {
    const categoryId = this.categories().length > 0 ? this.categories()[0].id : '00000000-0000-0000-0000-000000000000';
    const body = { ...this.newTask, categoryId, status: 'Open' };
    this.http.post(`${environment.apiUrl}/api/tasks`, body).subscribe({ next: () => { this.showTaskForm.set(false); this.loadAll(); }, error: () => {} });
  }
}
