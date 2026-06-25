import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-button',
  imports: [CommonModule],
  templateUrl: './button.html',
  styleUrl: './button.css',
})
export class Button {
  private router = inject(Router);

  @Input('hpSM') public hpSM: string = '16';
  @Input('hpMD') public hpMD: string = '20';

  @Input('wSM') public wSM: string = '';
  @Input('wMD') public wMD: string = '';

  @Input('textSizeSM') public textSizeSM: string = '';
  @Input('textSizeMD') public textSizeMD: string = '';

  @Input('color') public color: string = '';
  @Input('preset') public preset: string = '';

  @Input('routeLink') public routeLink: string = '';
  @Input('link') public link: string = '';

  @Output() onClick = new EventEmitter<MouseEvent>();

  ngOnInit() {
    //defaults
    this.textSizeSM = '3';
    this.textSizeMD = '4';

    this.hpSM = '12';
    this.hpMD = '16';

    let parts: string[] = this.preset.split(' ');

    for (let part of parts) {
      switch (part) {
        case 'full':
          this.wSM = 'full';
          this.wMD = 'full';
          break;

        case 'pink':
          this.color = 'bg-pink-300 hover:bg-pink-500 active:bg-pink-600 text-pink-500 text-white';
          break;

        case 'pinkalt':
          this.color = 'bg-pink-500 hover:bg-pink-400 active:bg-pink-600 text-pink-500 text-white';
          break;

        case 'pinkalt2':
          this.color =
            'bg-white hover:bg-pink-500 text-pink-500 hover:text-white border-4 border-pink-500';
          break;

        case 'blue':
          this.color = 'bg-white border-5 font-bold hover:bg-[#000555] hover:text-white';
          break;

        case 'bluealt':
          this.color = 'bg-[#000555] border-5 font-bold hover:bg-blue-400 text-white';
          break;

        case 't1':
          this.textSizeSM = '2';
          this.textSizeMD = '3';

          this.hpSM = '10';
          this.hpMD = '10';
          break;

        case 't2':
          this.textSizeSM = '3';
          this.textSizeMD = '4';

          this.hpSM = '12';
          this.hpMD = '14';
          break;

        case 't3':
          this.textSizeSM = '4';
          this.textSizeMD = '5';

          this.hpSM = '16';
          this.hpMD = '20';
          break;

        case 't4':
          this.textSizeSM = '4';
          this.textSizeMD = '6';
          this.hpSM = '50';
          this.hpMD = '40';
          break;
      }
    }
  }

  openRouteLink() {
    this.router.navigate([this.routeLink]);
  }

  openLink() {
    window.open(this.link, '_blank');
  }

  private parseColorFrom(partToRemove: string) {
    let otherPart: string = this.preset.replace(partToRemove, '');

    if (otherPart == 'pink') {
      this.color = 'bg-pink-300 hover:bg-pink-500 active:bg-pink-600 text-pink-500 text-white';
    }

    if (otherPart == 'pinkalt') {
      this.color = 'bg-pink-500 hover:bg-pink-400  active:bg-pink-600 text-pink-500 text-white';
    }

    if (otherPart == 'blue') {
      this.color =
        'bg-white border-5 font-bold h-[{{hp}}] hover: hover:bg-[#000555] hover:text-white';
    }
    if (otherPart == 'bluealt') {
      this.color = 'bg-[#000555] border-5 font-bold h-[{{hp}}] hover: hover:bg-blue-400 text-white';
    }
  }

  handleClick(event: MouseEvent): void {
    if (this.routeLink == '' && this.link == '') {
      this.onClick.emit();
    } else if (this.routeLink != '') {
      this.openRouteLink();
    } else if (this.link != '') {
      this.openLink();
    }
  }
}
