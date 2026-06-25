import { Component, Input } from '@angular/core';
import { SubmitInfo } from '../models/submit.model';
import { Block } from '../block/block';
import { Button } from '../button/button';

@Component({
  selector: 'app-submit-card',
  imports: [Block, Button],
  templateUrl: './submit-card.html',
  styleUrl: './submit-card.css',
})
export class SubmitCard {
  @Input() submitInfo!: SubmitInfo;

  openReview() {
    window.open(this.submitInfo.reviewLink, '_blank');
  }
}
