import { Signal } from '@angular/core';
import { ISelectable } from './iselectable.interface';

export interface ISearchService {
  search(v: string): Promise<void>;
  items: Signal<ISelectable[]>;
}
