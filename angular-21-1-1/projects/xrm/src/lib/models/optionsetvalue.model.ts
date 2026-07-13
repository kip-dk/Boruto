import { effect, signal } from "@angular/core";

export class OptionSetValue {

  static toOptionSetValue(a: any, f: string): OptionSetValue {
    if (a[f] == undefined) {
      return new OptionSetValue();
    }
    var result = new OptionSetValue();
    result.value = a[f] as number;
    result.name = a[f+"@OData.Community.Display.V1.FormattedValue"];

    delete a[f];
    delete a[f+"@OData.Community.Display.V1.FormattedValue"];
    delete a[f+"@OData.Community.Display.V1.AttributeName"];
    delete a[f+""];

    return result;
  }

    value?: number;
    name?: string;
    value$ = signal<number | undefined>(undefined);
    name$ = signal<string |undefined>(undefined);



    constructor(value?: number, name?: string) {
      this.value = value;
      this.name = name;

      this.value$.set(value);
      this.name$.set(name);

      effect(() => this.value = this.value$());
      effect(() => this.name = this.name$());
    }
  
    equals(o: OptionSetValue): boolean {
      if (this.value == null && (o == null || o.value == null)) return true;
      return this.value == o.value;
    }
  
    clone(): OptionSetValue {
      let r = new OptionSetValue();
      r.name = this.name;
      r.value = this.value;
      return r;
    }
  
    toJsonProperty(): any {
      return this.value;
    }
  
    static same(o1?: OptionSetValue, o2?: OptionSetValue): boolean {
      if (o1 == null && o2 == null) return true;
      let v1: number | null = null;
      let v2: number | null = null;
      if (o1 != null && o1.value != null) v1 = o1.value;
      if (o2 != null && o2.value != null) v2 = o2.value;
      return v1 == v2;
    }
  }
  