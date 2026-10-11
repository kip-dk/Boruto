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
  completeOnly?: boolean;
  icon?: string;
  expand?(): Promise<ISelectable[]>;

}


export interface ISelectableService {
    key: string;
    empty: boolean;
    items(): BehaviorSubject<ISelectable[]>;
    clearAllSelections(): void;
    toggled?(id: string): void;
}

export function enumToSelectable<T extends string | number>(
  enumType: Record<string, string | number>,
  icons?: Partial<Record<T, string>>
): ISelectable[] {
  return Object.entries(enumType)
    .filter(([key]) => isNaN(Number(key)))
    .map(([name, id]) => ({
      id: String(id),
      name,
      icon: icons?.[id as T]
    }));
}

export function selectableToEnum<T extends number | string>(items: ISelectable[]): T[] {
  return items.map(item => Number(item.id) as T);
}