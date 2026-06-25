import { Component, inject, Inject } from '@angular/core';
import { RouterModule, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { SubmitInfo } from '../models/submit.model';
import { FormsModule } from '@angular/forms';
import { Block } from '../block/block';
import { Button } from '../button/button';
import { SubmissionsList } from '../submissions-list/submissions-list';
import { DOCUMENT } from '@angular/common';

export enum SortingType {
  OldNew,
  NewOld,
}

@Component({
  selector: 'app-submissions',
  imports: [
    FormsModule,
    SubmissionsList,
    Button,
    Block,
    RouterModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './submissions.html',
})
export class Submissions {
  private http = inject(HttpClient);

  public submissions: SubmitInfo[] = [];

  public pendingSubmissions: SubmitInfo[] = [];
  public reviewedSubmissions: SubmitInfo[] = [];

  public pending: boolean = true;

  openPending() {
    this.pending = true;
  }
  openReviewed() {
    this.pending = false;
  }

  constructor(@Inject(DOCUMENT) private document: Document) {}

  ngOnInit() {
    this.document.body.classList.add('min-h-screen');
    this.document.body.classList.add('bg-[url(/bgmelted.png)]');
    this.document.body.classList.add('bg-repeat');

    this.http
      .get<SubmitInfo[]>('https://oknoweb.ru/submit/api/submissions')
      .subscribe((data: any) => {
        this.submissions = data;
        this.submissions.reverse();

        for (const submit of this.submissions) {
          if (submit.status == 'pending') {
            this.pendingSubmissions.push(submit);
          } else if (submit.status == 'reviewed') {
            this.reviewedSubmissions.push(submit);
          }
        }
        console.log(this.pendingSubmissions.length);
      });
  }

  ngOnDestroy() {
    this.document.body.classList.remove('min-h-screen');
    this.document.body.classList.remove('bg-[url(/bgmelted.png)]');
    this.document.body.classList.remove('bg-repeat');
  }
}
