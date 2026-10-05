import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CircleService } from '../../../core/services/circle.service';
import { AuthService } from '../../../core/services/auth.service';
import { CircleSummary } from '../../../core/models/circle.model';

@Component({
  selector: 'app-circle-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './circle-list.component.html',
  styleUrl: './circle-list.component.css'
})
export class CircleListComponent implements OnInit {
  private circleService = inject(CircleService);
  public authService = inject(AuthService);
  private cdr = inject(ChangeDetectorRef);

  circles: CircleSummary[] = [];
  filteredCircles: CircleSummary[] = [];
  isLoading = true;
  errorMessage = '';

  activeTab: 'available' | 'my' = 'available';
  searchTerm = '';
  selectedFrequency = 'ALL';

  ngOnInit(): void {
    this.loadCircles();
  }

  setTab(tab: 'available' | 'my'): void {
    if (this.activeTab === tab) return;
    this.activeTab = tab;
    this.loadCircles();
  }

  loadCircles(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request$ = this.activeTab === 'available'
      ? this.circleService.getAvailableCircles()
      : this.circleService.getMyCircles();

    request$.subscribe({
      next: (data) => {
        this.circles = data;
        this.applyFilters();
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.errorMessage = err.error?.message || err.error?.title || 'Failed to load savings circles.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  applyFilters(): void {
    this.filteredCircles = this.circles.filter(c => {
      const matchesSearch = !this.searchTerm.trim() ||
        c.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        c.organizerName.toLowerCase().includes(this.searchTerm.toLowerCase());

      const matchesFrequency = this.selectedFrequency === 'ALL' ||
        c.frequency.toUpperCase() === this.selectedFrequency.toUpperCase();

      return matchesSearch && matchesFrequency;
    });
  }

  getCapacityPercentage(circle: CircleSummary): number {
    if (!circle.memberLimit) return 0;
    return Math.min(100, Math.round((circle.memberCount / circle.memberLimit) * 100));
  }
}
