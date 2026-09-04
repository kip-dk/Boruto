import { NgClass } from '@angular/common';
import { Component, ElementRef, EventEmitter, input, Input, OnChanges, output, Output, SimpleChanges, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'xrmui-section',
  templateUrl: './section.xrmui.html',
  styleUrl: './section.xrmui.scss',
  imports: [FormsModule, MatIcon,NgClass]
})
export class XrmuiSection implements OnChanges {
  @ViewChild('searchelement') searchElement?: ElementRef;

  @Input('title') title: string = '';
  @Input('top-title') topTitle: string = '';
  @Input('searchable') searchable: boolean = false;
  @Input('search') search: string = '';
  @Input('placeholder') placeholder: string = ''
  @Output('searchChange') searchChange: EventEmitter<string> = new EventEmitter<string>();

  @Input('focusSearch') focusSearchTrigger: boolean = false;
  @Output('focusSearchChange') focusSearchTriggerChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  @Input('checkable') checkable: boolean = false;
  @Input('checked') checked: boolean = false;
  @Output('checkedChange') checkChange: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output('prev') prev: EventEmitter<void> = new EventEmitter<void>();
  @Output('next') next: EventEmitter<void> = new EventEmitter<void>();
  @Output('enter') enter: EventEmitter<void> = new EventEmitter();
  @Output('esc') esc: EventEmitter<void> = new EventEmitter();

  private release$ = 0;
  searchReleaseAfter = input<number | null>(null);
  searchReleased = output<void>();

  constructor() {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.focusSearchTrigger == true) {
      setTimeout(() => {
        this.focusSearch();
        this.focusSearchTriggerChange.emit(false);
      }, 10);
    }
  }

  searchChanged() {
    this.searchChange.emit(this.search);
    this.doRelease();
  }

  check() {
    this.checked = true;
    this.checkChange.emit(this.checked);
    this.focusSearch();
  }

  uncheck() {
    this.checked = false;
    this.checkChange.emit(this.checked);
    this.focusSearch();
  }

  onkeydown(e: Event) {
    if (this.prev.observed) {
      e.preventDefault();
      e.stopPropagation();
      this.prev.emit();
    }
  }

  onkeyup(e: Event) {
    if (this.next.observed) {
      e.preventDefault();
      e.stopPropagation();
      this.next.emit();
    }
  }

  onkeyenter(e: Event) {
    if (this.enter.observed) {
      e.preventDefault();
      e.stopPropagation();
      this.enter.emit();
    }
  }

  onkeyesc(e: Event) {
    if (this.esc.observed) {
      e.preventDefault();
      e.stopPropagation();
      this.esc.emit();
    }
  }

  private focusSearch() {
    if (this.searchElement != null) {
      this.searchElement.nativeElement.focus();
    }
  }

  private doRelease() {
    const re = this.searchReleaseAfter();
    if (re && re > 0) {
      if (this.release$ > 0) {
        clearTimeout(this.release$);
      }

      this.release$ = setTimeout(() => {
        this.searchReleased.emit();
      }, re);
    }
  }
}
