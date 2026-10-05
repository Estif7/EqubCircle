import { Component, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  // Flow State
  step: 'register' | 'fayda-otp' = 'register';
  isLoading = false;
  errorMessage = '';

  // Registered user context for OTP step
  registeredUserId = '';
  registeredPhone = '';
  devOtpHint: string | null = null;

  registerForm: FormGroup = this.fb.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{9,15}$/)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    faydaFan: ['', [Validators.required, Validators.minLength(10)]]
  });

  otpForm: FormGroup = this.fb.group({
    otp: ['', [Validators.required, Validators.pattern(/^[0-9]{6}$/)]]
  });

  onRegisterSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const req = {
      ...this.registerForm.value,
      // Normalize FAN (strip dashes/spaces)
      faydaFan: this.registerForm.value.faydaFan.replace(/[-\s]/g, '')
    };

    this.authService.register(req).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.registeredUserId = res.userId;
        this.registeredPhone = res.phoneNumber;
        this.devOtpHint = res.devSimulatedOtp || res.faydaOtpDevHint || null;
        this.step = 'fayda-otp';

        if (this.devOtpHint) {
          this.otpForm.patchValue({ otp: this.devOtpHint });
        }
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || err.error?.title || 'Registration failed. Please check your inputs and try again.';
        this.cdr.markForCheck();
      }
    });
  }

  onOtpSubmit(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.verifyFayda({
      userId: this.registeredUserId,
      otpCode: this.otpForm.value.otp
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.cdr.markForCheck();
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || err.error?.title || 'Invalid or expired OTP. Please try again.';
        this.cdr.markForCheck();
      }
    });
  }

  fillSampleFan(): void {
    this.registerForm.patchValue({
      faydaFan: '1234-5678-9012-3456'
    });
  }

  backToRegister(): void {
    this.step = 'register';
    this.errorMessage = '';
    this.cdr.markForCheck();
  }
}

