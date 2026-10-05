import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CircleListComponent } from './features/circles/circle-list/circle-list.component';
import { CircleDetailsComponent } from './features/circles/circle-details/circle-details.component';
import { CreateCircleComponent } from './features/circles/create-circle/create-circle.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'circles', component: CircleListComponent },
  { path: 'circles/create', component: CreateCircleComponent, canActivate: [authGuard] },
  { path: 'circles/:id', component: CircleDetailsComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', redirectTo: '/circles', pathMatch: 'full' },
  { path: '**', redirectTo: '/circles' }
];
