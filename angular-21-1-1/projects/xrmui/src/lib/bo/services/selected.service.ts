import { Injectable, signal, Signal, WritableSignal } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { ISelectable, ISelectableService } from '../api/iselectable.interface';

export interface SelectedListContainer {
  selected: string[],
  empty: boolean,
  loaded: boolean
}

export interface SelectedLists {
  [key: string]: BehaviorSubject<SelectedListContainer>
}

export interface NotSelectedLists {
  [key: string]: BehaviorSubject<boolean>
}

export interface LastSelectedItems {
  [key: string]: SelectedListContainer;
}

export interface ServiceBinds {
  [key: string]: ISelectableService;
}

export interface Counts {
  [key: string]: WritableSignal<number>;
}


@Injectable({providedIn:'root'})
export class SelectedService {

  private selectedItems: SelectedLists = {} as SelectedLists;
  private notSelectedItems: NotSelectedLists = {} as NotSelectedLists;
  private lastSelectedItems: LastSelectedItems = {} as LastSelectedItems;
  private binds: ServiceBinds = {} as ServiceBinds;
  private keys: string[] = [];
  private nots: string[] = [];
  private counts: Counts = {};

  private resetFilter$: Subject<void>;

  constructor() {
    this.resetFilter$ = new Subject<void>();
  }

  resetAll() {
    this.resetFilter$.next();
  }

  reset(): Subject<void> {
    return this.resetFilter$;
  }

  get(key: string): BehaviorSubject<SelectedListContainer> {
    if (this.selectedItems[key] != null) {
      return this.selectedItems[key];
    }
    this.selectedItems[key] = new BehaviorSubject<SelectedListContainer>({ selected: [], empty: false, loaded: false });
    this.lastSelectedItems[key] = { selected: [], empty: false, loaded: false };
    this.keys.push(key);
    return this.selectedItems[key];
  }

  count(key: string): Signal<number> {
    if (this.counts[key] != null) {
      return this.counts[key];
    }

    const con = this.get(key);
    this.counts[key] = signal<number>(con.value.selected.length);
    return this.counts[key];
  }

  bind(service: ISelectableService): void {
    this.binds[service.key] = service;
  }

  not(key: string): BehaviorSubject<boolean> {
    if (this.notSelectedItems[key] != null) {
      return this.notSelectedItems[key];
    }
    this.notSelectedItems[key] = new BehaviorSubject<boolean>(false);
    this.nots.push(key);
    return this.notSelectedItems[key];
  }

  push(key: string, values: string[]) {
    const selector = this.binds[key];
    if (selector != null && selector != undefined) {
      selector.items().value.forEach(r => {
        r.selected = values.find(v => v == r.id) != null;
      });
      const tobepublished = selector.items().value.filter(r => r.selected == true).map(r => r.id);
      this.publish(key, tobepublished, false);
    } else {
      throw '[' + key + ']' + ' er ikke blevet bundet til service';
    }
  }

  publish(key: string, values: string[], empty: boolean) {
    var beh = this.get(key);
    var value = beh.value;
    value.selected = values;
    value.empty = empty;

    var las = this.lastSelectedItems[key];

    if (!las.loaded || !this.isSame(value, las)) {
      las.loaded = true;
      las.empty = value.empty;
      las.selected = value.selected;
      beh.next(value);
    }

    if (this.counts[key] != null) {
      this.counts[key].set(values.length);
    }
  }

  silentUnselectAll(items?: BehaviorSubject<ISelectable[]>) {
    if (items != undefined) {
      items.value.forEach(r => { r.selected = false;  });
    }
  }

  toggleNot(k: string) {
    const n = this.not(k);
    const next = !n.value;
    n.next(next);
  }

  clearAllNots() {
    this.nots.forEach(n => {
      if (this.notSelectedItems[n].value == true) {
        this.notSelectedItems[n].next(false);
      }
    });
  }

  private isSame(next: SelectedListContainer,last: SelectedListContainer): boolean {
    if (next.empty != last.empty) return false;
    if (next.selected.length != last.selected.length) return false;

    for (var i=0;i<next.selected.length;i++) {
      if (next.selected[i] != last.selected[i]) {
        return false;
      }
    }
    return true;
  } 
}
