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
  @Input('hpXL') public hpXl: string = '20';

  @Input('wSM') public wSM: string = '';
  @Input('wMD') public wMD: string = '';

  @Input('textSizeSM') public textSizeSM: string = '';
  @Input('textSizeMD') public textSizeMD: string = '';
  @Input('textSizeXL') public textSizeXL: string = '';

  @Input('color') public color: string = '';
  @Input('preset') public preset: string = '';

  @Input('target') public target: string = '_blank';
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
          this.color = 'bg-pink-400 hover:bg-pink-500 active:bg-pink-600 text-pink-500 text-white';
          break;

        case 'pinkalt':
          this.color = 'bg-pink-500 hover:bg-pink-400 active:bg-pink-600 text-white';
          break;

        case 'pinkalt2':
          this.color =
            'bg-white hover:bg-pink-500 text-pink-500 hover:text-white border-4 border-pink-500';
          break;

        case 'blue':
          this.color =
            'font-bold bg-white text-blue-700 border-4 3xl:border-8 border-blue-700 hover:bg-blue-700 hover:text-white';
          break;

        case 'bluealt':
          this.color = 'font-bold bg-[#000555] border-5 hover:bg-blue-400 text-white';
          break;

        case 't1':
          this.textSizeSM = '2xl';
          this.textSizeMD = '3xl';
          this.textSizeXL = '4xl';

          this.hpSM = '10';
          this.hpMD = '10';
          break;

        case 't2':
          this.textSizeSM = '3xl';
          this.textSizeMD = '4xl';
          this.textSizeXL = '6xl';

          this.hpSM = '12';
          this.hpMD = '14';
          break;

        case 't3':
          this.textSizeSM = '4xl';
          this.textSizeMD = '5xl';
          this.textSizeXL = '6xl';

          this.hpSM = '16';
          this.hpMD = '20';
          break;

        case 't4':
          this.textSizeSM = '4xl';
          this.textSizeMD = '6xl';
          this.textSizeXL = '7xl';

          this.hpSM = '30';
          this.hpMD = '40';
          break;

        case 't5':
          this.textSizeSM = '5xl';
          this.textSizeMD = '7xl';
          this.textSizeXL = '8xl';

          this.hpSM = '50';
          this.hpMD = '40';
          break;

        case 'tvw':
          this.textSizeSM = '[2vw]';
          this.textSizeMD = '[2vw]';
          this.textSizeXL = '[2vw]';

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
