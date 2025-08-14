import { Component, OnInit, signal } from '@angular/core';
// CommonModule більше не потрібен для if/for!
// import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UrlApiService } from './url-api.service';
import { UrlListItemDto, UrlsPageDto } from './models';

@Component({
  selector: 'urls-app-root',
  standalone: true,
  imports: [FormsModule],
  template: `
  <div class="row">
    <div class="col-lg-8">
      <table class="table table-striped align-middle">
        <thead>
          <tr>
            <th>Original URL</th>
            <th>Short</th>
            <th style="width: 180px;">Actions</th>
          </tr>
        </thead>
        <tbody>
          <!-- 2. Замінюємо *ngFor на новий синтаксис @for -->
          <!-- track u.id є обов'язковим і значно покращує продуктивність -->
          @for (u of items(); track u.id) {
            <tr>
              <td class="text-break">{{ u.originalUrl }}</td>
              <td><a [href]="'/u/' + u.shortCode" target="_blank">{{ u.shortCode }}</a></td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-2"
                        [disabled]="!isAuthorized()"
                        (click)="goDetails(u.id)">
                  Details
                </button>
                <button class="btn btn-sm btn-danger"
                        [disabled]="!u.canDelete"
                        (click)="remove(u.id)">
                  Delete
                </button>
              </td>
            </tr>
          } @empty {
            <tr>
              <td colspan="3" class="text-center">No URLs yet.</td>
            </tr>
          }
        </tbody>
      </table>
    </div>

    <div class="col-lg-4">
      <!-- 4. Замінюємо *ngIf/else на більш читабельний синтаксис @if/@else -->
      @if (isAuthorized()) {
        <div class="card">
          <div class="card-body">
            <h5 class="card-title">Add new Url</h5>
            <form (ngSubmit)="add()">
              <input class="form-control" [(ngModel)]="newUrl" name="originalUrl" placeholder="https://..." />
              <button class="btn btn-primary mt-2" [disabled]="!newUrl.trim()">Add</button>
            </form>

            @if (error()) {
              <div class="text-danger mt-2">{{ error() }}</div>
            }
          </div>
        </div>
      } @else {
        <div class="alert alert-info">Login to add URLs and view details.</div>
      }
    </div>
  </div>
  `
})
export class AppComponent implements OnInit {
  items = signal<UrlListItemDto[]>([]);
  isAuthorized = signal(false);
  isAdmin = signal(false);
  newUrl = '';
  error = signal('');

  constructor(private api: UrlApiService) {}

  ngOnInit(): void { this.load(); }

  load() {
    this.api.getAll().subscribe({
      next: (r: UrlsPageDto) => {
        this.items.set(r.items);
        this.isAuthorized.set(r.isAuthorized);
        this.isAdmin.set(r.isAdmin);
      },
      error: () => this.error.set('Failed to load data')
    });
  }

  add() {
    const url = this.newUrl.trim();
    if (!url) return;
    this.error.set('');
    this.api.create(url).subscribe({
      next: (created) => {
        this.items.update(arr => [created, ...arr]);
        this.newUrl = '';
      },
      error: (e) => {
        const msg = (e?.error && typeof e.error === 'string') ? e.error : 'Create failed';
        this.error.set(msg);
      }
    });
  }

  remove(id: number) {
    this.api.delete(id).subscribe({
      next: () => this.items.update(arr => arr.filter(x => x.id !== id)),
      error: () => this.error.set('Delete failed')
    });
  }

  goDetails(id: number) {
    if (!this.isAuthorized()) return;
    window.location.href = `/Urls/Details/${id}`;
  }
}
