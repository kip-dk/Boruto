import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'ko-expansion-panel',
  templateUrl: './koExpansionPanel.component.html',
  styleUrls: ['./koExpansionPanel.component.css']
})
export class KoExpansionPanelComponent {

  @Input('expanded') expanded: boolean = false;
  @Output('expandedChange') expandedChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() {
  }

  toggle(): void {
    this.expanded = !this.expanded;
    this.expandedChange.emit(this.expanded);
  }
}
