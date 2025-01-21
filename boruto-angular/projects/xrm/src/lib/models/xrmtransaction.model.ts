import { Entity } from "./entity.model";
import { XrmTransactionItem } from "./xrmtransactionitem.model";

export class XrmTransaction {
    private oprs: XrmTransactionItem[] = [];
  
    put<T extends Entity>(prototype: T, instance: T, field: string, value: any): void {
      this.oprs.push(new XrmTransactionItem("put", prototype, instance, field, value));
    }
  
    delete<T extends Entity>(instance: T): void {
      this.oprs.push(new XrmTransactionItem("delete", instance, instance));
    }
  
    create<T extends Entity>(prototype: T, instance: T): void {
      this.oprs.push(new XrmTransactionItem("create", prototype, instance));
    }
  
    update<T extends Entity>(prototype: T, instance: T): void {
      this.oprs.push(new XrmTransactionItem("update", prototype, instance));
    }

    length(): number {
      return this.oprs.length;
    }

    clear(): void {
      this.oprs = [];
    }
  }
  