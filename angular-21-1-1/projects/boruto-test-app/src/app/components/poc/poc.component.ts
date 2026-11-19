import { Component, ChangeDetectionStrategy } from '@angular/core';
import { XrmuiGreedy } from 'boruto-xrmui';

@Component({
  selector: 'app-poc',
  templateUrl: './poc.component.html',
  styleUrl: './poc.component.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [XrmuiGreedy]
})
export class PocComponent {
}
