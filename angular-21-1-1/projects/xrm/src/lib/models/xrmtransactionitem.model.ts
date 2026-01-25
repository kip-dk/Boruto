import { Entity } from "./entity.model";

export class XrmTransactionItem {
    constructor(type: string, prototype: Entity, instance: Entity, field?: string, value?: any) {
      this.type = type;
      this.prototype = prototype;
      this.instance = instance;
      this.field = field;
      this.value = value;
    }
  
    type: string;
    prototype: Entity;
    instance: Entity;
    field?: string;
    value: any;
    id: number | null = null;
  }
  