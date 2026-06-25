import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Submit } from './submit/submit';
import { Submissions } from './submissions/submissions';
import { SubmitSuccess } from './submit-success/submit-success';

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  { path: 'home', component: Home },
  { path: 'submissions', component: Submissions },
  { path: 'submit', component: Submit },
  { path: 'submit/success/:id', component: SubmitSuccess },
];
