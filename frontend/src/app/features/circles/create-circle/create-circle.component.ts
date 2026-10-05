import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CircleService } from '../../../core/services/circle.service';

@Component({
  selector: 'app-create-circle',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './create-circle.component.html',
  styleUrl: './create-circle.component.css'
})
export class CreateCircleComponent {
  private fb = inject(FormBuilder);
  private circleService = inject(CircleService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  isLoading = false;
  errorMessage = '';

  circleForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    contributionAmount: [1000, [Validators.required, Validators.min(10), Validators.max(10000000)]],
    frequency: ['MONTHLY', [Validators.required]],
    memberLimit: [5, [Validators.required, Validators.min(2), Validators.max(500)]]
  });

  get targetPot(): number {
    const amount = this.circleForm.get('contributionAmount')?.value || 0;
    const limit = this.circleForm.get('memberLimit')?.value || 0;
    return amount * limit;
  }

  onSubmit(): void {
    if (this.circleForm.invalid) {
      this.circleForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.circleService.createCircle(this.circleForm.value).subscribe({
      next: (createdCircle) => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.router.navigate(['/circles', createdCircle.id]);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || err.error?.title || 'Failed to create savings circle. Please verify your inputs.';
        this.cdr.markForCheck();
      }
    });
  }
}
