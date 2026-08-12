import { BehaviorSubject } from "rxjs";
import { OptionSetValue } from "./optionsetvalue.interface";

export interface ISelectable {
  id: string;
  name?: string;
  selected?: boolean;
  disabled?: boolean;
  badget?: number;
  statecode?: OptionSetValue;
  group?: string;
  color?: string;
  latitude?: number;
  longitude?: number;
  coorcolor?: string;
  expandable?: boolean;
  expand?(): Promise<ISelectable[]>;

}


export interface ISelectableService {
    key: string;
    empty: boolean;
    items(): BehaviorSubject<ISelectable[]>;
    clearAllSelections(): void;
    toggled?(id: string): void;
  }