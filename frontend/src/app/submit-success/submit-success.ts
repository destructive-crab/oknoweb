import { Component, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Button } from '../button/button';
import { Block } from '../block/block';

@Component({
  selector: 'app-submit-success',
  imports: [Button, Block],
  templateUrl: './submit-success.html',
  styleUrl: './submit-success.css',
})
export class SubmitSuccess {
  public id: string = 'invalid';

  constructor(
    private route: ActivatedRoute,
    @Inject(DOCUMENT) private document: Document,
  ) {
    this.route.params.subscribe((params) => {
      this.id = params['id'];
      // Use the id to fetch specific data
    });
  }
  ngOnInit() {
    this.document.body.classList.add('min-h-screen');
    this.document.body.classList.add('bg-[url(/bgmeltedpink.png)]');
    this.document.body.classList.add('bg-repeat');
  }

  ngOnDestroy() {
    this.document.body.classList.remove('min-h-screen');
    this.document.body.classList.remove('bg-[url(/bgmeltedpink.png)]');
    this.document.body.classList.remove('bg-repeat');
  }
}
