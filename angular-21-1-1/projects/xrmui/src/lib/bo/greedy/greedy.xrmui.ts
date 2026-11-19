import {
  Component,
  ElementRef,
  inject,
  signal,
  AfterViewInit,
  OnDestroy,
  ChangeDetectionStrategy
} from '@angular/core';

@Component({
  selector: 'xrmui-greedy',
  standalone: true,
  template: '<ng-content />',
  styleUrl: './greedy.xrmui.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '[style.height.px]': 'height()'
  }
})
export class XrmuiGreedy implements AfterViewInit, OnDestroy {
  private element = inject(ElementRef<HTMLElement>);

  height = signal(0);

  private resizeObserver?: ResizeObserver;

  private updateHeight = () => {
    const top = this.element.nativeElement.getBoundingClientRect().top;
    this.height.set(Math.max(0, window.innerHeight - top - 15));
  };

  ngAfterViewInit(): void {
    this.updateHeight();

    window.addEventListener('resize', this.updateHeight);

    this.resizeObserver = new ResizeObserver(this.updateHeight);
    this.resizeObserver.observe(document.body);
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.updateHeight);
    this.resizeObserver?.disconnect();
  }
}