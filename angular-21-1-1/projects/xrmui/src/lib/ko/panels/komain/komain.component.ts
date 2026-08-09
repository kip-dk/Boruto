import { Component, ViewEncapsulation, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'ko-main',
  templateUrl: './komain.component.html',
  styleUrls: ['./komain.component.scss'],
  imports: [],
  changeDetection: ChangeDetectionStrategy.Eager,
  encapsulation: ViewEncapsulation.None


})
export class KoMainComponent {
  constructor() {
  }
}
