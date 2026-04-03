import { Component, effect, input, output, signal, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'ko-expansion-panel',
  templateUrl: './koexpansionpanel.component.html',
  styleUrls: ['./koexpansionpanel.component.scss'],
  imports: [],
  encapsulation: ViewEncapsulation.None
})
export class KoExpansionPanelComponent {

  expanded = input<boolean>(false);
  expandedChange = output<boolean>();

  isExpanded = signal<boolean>(false);

  constructor() {
    effect(() => {
      this.isExpanded.set(this.expanded());
    });
  }

  toggle(): void {
    this.isExpanded.set(!this.isExpanded());
    this.expandedChange.emit(this.isExpanded());
  }
}
