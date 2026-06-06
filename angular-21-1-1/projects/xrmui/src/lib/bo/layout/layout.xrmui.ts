import { CdkDrag } from '@angular/cdk/drag-drop';
import { NgClass } from '@angular/common';
import { AfterViewInit, Component, computed, effect, ElementRef, inject, input, OnDestroy, signal } from '@angular/core';
import { XrmuiPanel } from '../models/xrmuipanel.model';
import { IDistance } from '../api/idistance.interface';


interface IStyle {
    style: string;
    dyn: boolean;
}

@Component({
    selector: 'xrmui-layout',
    templateUrl: './layout.xrmui.html',
    styleUrl: './layout.xrmui.scss',
    imports: [CdkDrag,NgClass]
})
export class XrmuiLayout implements AfterViewInit, OnDestroy {

    _panels = input<string>('', { alias: 'panels' });
    _orientation = input<'horizontal' | 'vertical'>('horizontal', {alias: 'orientation'});
    _resizeable = input<boolean>(false, { alias:'resizeable' });

    panels = signal<string>('');
    orientation = signal<'horizontal' | 'vertical'>('horizontal');
    resizeable = signal<boolean>(false);


    private totalHeight: number = 0;
    private totalWidth: number = 0;

    private unit: 'px' | '%' = 'px';

    private panellist$ = signal<XrmuiPanel[]>([]);
    private resized = signal<number>(0);
    panellist = this.panellist$.asReadonly();

    private viewInitialized: boolean = false;

    style0 = computed(() => { return this.styleFor(0) });
    style1 = computed(() => { return this.styleFor(1) });
    style2 = computed(() => { return this.styleFor(2) });
    style3 = computed(() => { return this.styleFor(3) });
    style4 = computed(() => { return this.styleFor(4) });

    private styleFor(i: number) : IStyle {
        const panels = this.panellist();
        const resize = this.resized();


        if (panels.length > i) {
            return { style: panels[i].style, dyn: panels[i].dyn }
        }
        return { style: "display: none;", dyn: false }
    }


    private element: ElementRef = inject(ElementRef);
    
  constructor() {
    effect(() => {
        this.panels.set(this._panels());
        this.orientation.set(this._orientation());
        this.resizeable.set(this._resizeable());

        if (this.viewInitialized) {
            // this.render();
        }
    })
  }

  private resizeObserver?: ResizeObserver;

  ngAfterViewInit(): void {
    this.resizeObserver = new ResizeObserver(entries => {
        const width = entries[0].contentRect.width;
    
        if (width > 0) {
          this.render();
          this.viewInitialized = true;
        }
      });
    
      this.resizeObserver.observe(this.element.nativeElement);  
}

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
      
  }
  
  private render(): void {
    this.totalWidth = this.element.nativeElement.offsetWidth;
    this.totalHeight = this.element.nativeElement.clientHeight;

    const nextpanellist: XrmuiPanel[] = [];

    // If panels are resizeable, we will work in px only
    if (this.resizeable() && this.panels() != null && this.panels().length > 1) {
        const ps: string[] = [];
        let used = 0;
        this.panels().split(',').forEach(r => {
            if (r == '?') {
                ps.push(r);
                return;
            }

            if (r.indexOf('px') >= 0) {
                used += this.sizeToNumber(r);
                ps.push(r);
                return;
            }
            const pct = this.sizeToNumber(r);
            if (this.orientation() == 'vertical') {
                const next = Math.round(this.totalWidth * pct / 100);
                used += next;
                ps.push(next.toString() + 'px');
            } else {
                const next = Math.round(this.totalHeight * pct / 100);
                used += next;
                ps.push(next.toString() + 'px');
            }
        });

        let left = this.totalHeight - used;
        if (this.orientation() == 'vertical') {
            left = this.totalWidth - used;
        }

        var freeIndex = ps.indexOf('?');
        if (freeIndex >= 0 && freeIndex < (ps.length - 1)) {
            ps[freeIndex] = left.toString() + 'px';
            ps[ps.length - 1] = '?';
        }

        this.panels.set(ps.join(','));
    }

    if (this.panels().length > 0) {
        var numbers = this.getNumbers();
        var countOpen = numbers.filter(r => r == -1).length;

        if (numbers.length > 5) {
            throw new Error("A maximum of 5 containers is supported");
        }

        var bind: 'top' | 'bottom' = 'top';
        var currentcursor: number = 0;

        if (countOpen > 1) {
            throw new Error('Only one open container is supported');
        }

        for (var i=0;i<numbers.length;i++) {
            var num = numbers[i];
            var next = new XrmuiPanel();
            next.index = i;
            next.orientation = this.orientation();

            if (num > 0 && bind == 'top') {

                if (this.orientation() == 'horizontal') {
                    next.top = currentcursor.toString() + this.toUnit(currentcursor);
                    next.height = num.toString() + this.unit;
                } else {
                    next.left = currentcursor.toString() + this.toUnit(currentcursor);
                    next.width = num.toString() + this.unit;
                }
                currentcursor += num;
                nextpanellist.push(next);
                continue;
            }

            if (num == -1) {
                var sum = this.sumAfter(numbers, i);
                if (this.orientation() == 'horizontal') {
                    next.top = currentcursor.toString() + this.toUnit(currentcursor);
                    next.bottom = sum.toString() + this.toUnit(sum);
                } else {
                    next.left = currentcursor.toString() + this.toUnit(currentcursor);
                    next.right = sum.toString() + this.toUnit(sum);
                }
                next.dyn = true;
                nextpanellist.push(next);
                bind = 'bottom';
                continue;
            }

            if (num >= 0 && bind == 'bottom') {
                var sum = this.sumAfter(numbers,i);
                if (this.orientation() == 'horizontal') {
                    next.bottom = sum.toString() + this.toUnit(sum);
                    next.height = num.toString() + this.unit;
                } else {
                    next.right = sum.toString() + this.toUnit(sum);
                    next.width = num.toString() + this.unit;
                }
                nextpanellist.push(next);
                continue;
            }
            throw new Error("Unexpected handle of elements");
        }

        nextpanellist.forEach(p => p.render());
        this.panellist$.set(nextpanellist);
    }
  }

  resize(e: any, panel: XrmuiPanel) {
    const d = e['distance'] as IDistance;
    if (panel.orientation == 'vertical') 
    {
        panel.moveTo(d.x, this.panellist$()[panel.index + 1]);
    }

    if (panel.orientation == 'horizontal') {
        panel.moveTo(d.y, this.panellist$()[panel.index + 1]);
    }

    const r = this.resized();
    this.resized.set(r + 1);
  }

  drop(e: any, panel: XrmuiPanel) {
    panel.endResize();
  }

  private toUnit(n: number): string {
    if (n > 0) {
        return this.unit;
    }
    return '';
  }

  private getNumbers(): number[] {
    this.unit = 'px';
    var result = [] as number[];
    if (this.panels().length > 0) {
        var unitAssigned = false;
        this.panels().split(',').forEach(r => {
            if (r.indexOf('px') >= 0) {
                if (unitAssigned && this.unit != 'px') {
                    throw new Error("unit can only be assigned once and must be consistent. Unit already set to " + this.unit);
                }
                this.unit = 'px';
                unitAssigned = true;
            }

            if (r.indexOf('%') >= 0) {
                if (unitAssigned && this.unit != '%') {
                    throw new Error("unit can only be assigned once and must be consistent. Unit already set to " + this.unit);
                }
                this.unit = '%';
                unitAssigned = true;
            }
            if (r == '?') {
                result.push(-1);
            } else {
                result.push(this.sizeToNumber(r));
            }
        });
    }
    return result;
  }

  private sizeToNumber(r: string) {
    return Number(r.replace('px','').replace('%',''));
  }


  private sumAfter(numbers: number[], index: number): number {
    var result = 0;
    for (var i=index+1;i<numbers.length;i++) {
        result += numbers[i];
    }
    return result;
  }
}
