import { NgClass } from '@angular/common';
import { Component, input, output, OnInit, OnDestroy, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import { MatBadge } from '@angular/material/badge';
import { ISelectable, ISelectableService } from '../api/iselectable.interface';
import { SelectedService } from '../services/selected.service';


export interface IGroupedSelectable {
  group?: string;
  items: ISelectable[];
}

@Component({
    selector: 'xrmui-info',
    templateUrl: './info.xrmui.html',
    styleUrl: './info.xrmui.scss',
    imports: [MatIcon,MatBadge]
})
export class XrmuiInfo implements OnInit, OnDestroy  {

  private selectedService: SelectedService = inject(SelectedService);

    label = input<string>();
    onlyshowselected = input<boolean>(false);
    items = input<ISelectableService>();
    icon = input<string>();
    click = output<void>();
    clear = output<void>();
    badget = input<number>();

    list$?: Subscription;
    list: ISelectable[] = [];

    select$?: Subscription;

    grouped: IGroupedSelectable[] = [];

    over: ISelectable | null = null;

    isnot: boolean = false;
    isnot$?: Subscription;

  constructor() {
  }

  ngOnInit() {
    const service = this.items();

    if (service != null) {
      this.select$ = this.selectedService.get(service.key).subscribe(v => {
        this.list = this.list.filter(v => v.selected == true);
      });

      this.list$ = service.items().subscribe(r => {
        this.list = r.filter(v => v.selected == true);
        this.doGrouping();
      });
    } else {
      this.list$?.unsubscribe();
      this.select$?.unsubscribe();
      this.isnot$?.unsubscribe();
      this.list = [];
      this.doGrouping();
    }
  }

  ngOnDestroy(): void {
    this.list$?.unsubscribe();
    this.isnot$?.unsubscribe();
    this.select$?.unsubscribe();
  }

  onClick() {
    this.click.emit();
  }

  doClear() {
    this.clear.emit();
  }

  isover(v: ISelectable) {
    this.over = v;
  }

  isout() {
    this.over = null;
  }

  remove(e: Event, v: ISelectable) {
    e.stopImmediatePropagation();
    const it = this.items();
    if (it) {
      v.selected = false;
      this.list = this.list.filter(r => r.selected == true);
      const sn = this.list.filter(r => r.selected == true).map(r => r.id);
      this.selectedService.publish(it.key, sn, false);
      this.doGrouping();
    }
  }

  private doGrouping() {
    this.grouped = [];
    let lastGroup: IGroupedSelectable | undefined = undefined; 
    const result: IGroupedSelectable[] = [];

    this.list.forEach(v => {
      if (v.selected == true) {
        if (lastGroup == undefined) {
          lastGroup = { group: v.group, items: [v] };
          result.push(lastGroup);
          return;
        }
        if (lastGroup.group == v.group) {
          lastGroup.items.push(v);
          return;
        }

        lastGroup = { group: v.group, items: [v] };
        result.push(lastGroup);
      }
    });
    this.grouped = result;
  }
}
