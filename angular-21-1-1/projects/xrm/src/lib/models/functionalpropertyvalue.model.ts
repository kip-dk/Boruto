import { ToFunctionPropertyValue } from "./tofunctionalpropertyvalue.interface";

export class FunctionPropertyValue implements ToFunctionPropertyValue {
    private v: string = '';
    constructor(v: string) {
      this.v = v;
    }
  
    functionPropertyValueAsString(): string {
      return this.v;
    }
  }
  