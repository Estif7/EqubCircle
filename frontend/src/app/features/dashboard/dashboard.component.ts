import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);

  public currentUser$ = this.authService.currentUser$;
  public userDashboard: any = null;
  public isLoading = true;
  public errorMessage = '';

  ngOnInit(): void {
    this.fetchUserDashboard();
  }

  fetchUserDashboard(): void {
    this.isLoading = true;
    this.http.get(`${environment.apiUrl}/dashboard/my`).subscribe({
      next: (data) => {
        this.userDashboard = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || err.error?.title || 'Could not load dashboard data.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
