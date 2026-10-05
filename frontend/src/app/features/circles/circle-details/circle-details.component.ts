import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CircleService } from '../../../core/services/circle.service';
import { AuthService } from '../../../core/services/auth.service';
import { CircleDetails, CircleMember } from '../../../core/models/circle.model';

@Component({
  selector: 'app-circle-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './circle-details.component.html',
  styleUrl: './circle-details.component.css'
})
export class CircleDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private circleService = inject(CircleService);
  public authService = inject(AuthService);

  circleId = '';
  circle: CircleDetails | null = null;
  isLoading = true;
  isJoining = false;
  isStarting = false;
  errorMessage = '';
  successMessage = '';

  get currentUserMembership(): CircleMember | undefined {
    if (!this.circle || !this.authService.currentUser) return undefined;
    return this.circle.members.find(m => m.userId === this.authService.currentUser?.id);
  }

  get canJoin(): boolean {
    if (!this.circle || !this.authService.isAuthenticated()) return false;
    return this.circle.status === 'OPEN' &&
      !this.circle.isMember &&
      this.circle.memberCount < this.circle.memberLimit;
  }

  ngOnInit(): void {
    this.circleId = this.route.snapshot.paramMap.get('id') || '';
    if (this.circleId) {
      this.loadCircleDetails();
    }
  }

  loadCircleDetails(): void {
    this.isLoading = true;
    this.circleService.getCircleDetails(this.circleId).subscribe({
      next: (data) => {
        this.circle = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to load circle details.';
        this.isLoading = false;
      }
    });
  }

  onJoinCircle(): void {
    if (!this.canJoin) return;

    this.isJoining = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.circleService.joinCircle(this.circleId).subscribe({
      next: (membership) => {
        this.isJoining = false;
        this.successMessage = 'Congratulations! You have successfully joined this savings circle.';
        this.loadCircleDetails();
      },
      error: (err) => {
        this.isJoining = false;
        this.errorMessage = err.error?.message || 'Could not join circle. Please try again.';
      }
    });
  }

  onStartCircle(): void {
    if (!this.circle || !this.circle.isOrganizer || this.circle.status !== 'OPEN') return;

    this.isStarting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.circleService.startCircle(this.circleId).subscribe({
      next: () => {
        this.isStarting = false;
        this.successMessage = 'Circle started! Rounds and payout orders have been generated.';
        this.loadCircleDetails();
      },
      error: (err) => {
        this.isStarting = false;
        this.errorMessage = err.error?.message || 'Could not start circle.';
      }
    });
  }
}
