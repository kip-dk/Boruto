import { Comparator } from "./comparator.enum";
import { Entity } from "./entity.model";
import { EntityReference } from "./entityreference.model";
import { IndexedObject } from "./indexedobject.model";
import { OptionSetValue } from "./optionsetvalue.model";

export class Filter {
    field?: string;
    operator?: Comparator;
    alias?: string;
    value: any;
    raw: boolean = false;
  
    constructor() {
    }
  
    toQueryString(prototype: Entity): string {
      if (this.raw == true && this.field != null) {
        return this.field;
      }
  
      if (this.operator == 100) {
        return 'ownerid eq-useroruserhierarchy';
      }
  
      if (this.operator == 101) {
        return 'ownerid eq-userteams';
      }
  
      let result = '';
      let _f = this.field;
  
      if (this.operator == Comparator.Equals && this.value == null) {
        // this.operator = Comparator.DoesNotContainsData;
      }
  
      if (this.operator == Comparator.NotEquals && this.value == null) {
        // this.operator = Comparator.ContainsData;
      }
  
      switch (this.operator) {
        case Comparator.ContainsData: {
          return _f + ' ne null';
        }
        case Comparator.DoesNotContainsData: {
          return _f + ' eq null';
        }
      }
  
      if (this.value == null) {
        throw new Error("value is required for operation type " + this.operator);
      }
  
      let _v = "'" + this.value + "'";
  
      if (typeof this.value == 'number') {
        _v = this.value.toString();
      }
  
      if (typeof this.value == 'boolean') {
        _v = this.value.valueOf() ? 'true' : 'false';
      }
  
      var pt = prototype as IndexedObject;
  
      if (this.field != null && pt[this.field] instanceof OptionSetValue) {
        if (this.value != null && this.value.hasOwnProperty('value')) {
          _v = this.value.value;
        }
      }
  
  
      let isEref = false;
      if (this.field != null && pt[this.field] instanceof EntityReference) {
        _f = "_" + this.field + "_value";
        if (this.value != null) {
          if (typeof this.value == 'string') {
            _v = this.value.replace('{', '').replace('}', '');
          } else {
            _v = this.value.id.replace('{', '').replace('}', '');
          }
        }
        isEref = true;
      }
  
      if (!isEref && _f != null && _f.startsWith('_') && _f.endsWith('_value') && _v != null && this.value != null) {
        if (typeof this.value === "string") {
          _v = this.value.replace('{', '').replace('}', '');
        } else {
          if (this.value.hasOwnProperty("id")) {
            _v = this.value.id.replace('{', '').replace('}', '');
          }
        }
      }
  
      var isDate = false;
      if (this.field != null && pt[this.field] instanceof Date) {
        if (this.value instanceof Date) {
          _v = this.value.toISOString();
        } else {
          _v = this.value.toString();
        }
        isDate = true;
      }
  
      if (!isDate && this.value instanceof Date) {
        _v = this.value.toISOString();
        isDate = true;
      }
  
      if (_f == prototype._keyName) {
        _v = this.value.replace('{', '').replace('}', '');
      }
  
      if (_v != null && _v != '') {
        _v = encodeURIComponent(_v);
      }
  
      switch (this.operator) {
        case Comparator.Equals: {
          return _f + ' eq ' + _v;
        }
        case Comparator.NotEquals: {
          return _f + ' ne ' + _v;
        }
        case Comparator.GreaterThan: {
          return _f + ' gt ' + _v;
        }
        case Comparator.GreaterThanOrEqual: {
          return _f + ' ge ' + _v;
        }
        case Comparator.LessThan: {
          return _f + ' lt ' + _v;
        }
        case Comparator.LessThanOrEQual: {
          return _f + ' le ' + _v;
        }
        case Comparator.Contains: {
          return "contains(" + _f + "," + _v + ")";
        }
        case Comparator.NotContains: {
          return "not contains(" + _f + "," + _v + ")";
        }
        case Comparator.StartsWith: {
          return "startswith(" + _f + "," + _v + ")";
        }
        case Comparator.NotStartsWith: {
          return "not startswith(" + _f + "," + _v + ")";
        }
        case Comparator.EndsWith: {
          return "endswith(" + _f + "," + _v + ")";
        }
        case Comparator.NotEndsWith: {
          return "not endswith(" + _f + "," + _v + ")";
        }
      }
      return result;
    }
  
    toFetcmXml(): string {
      if (this.operator == 100) {
        return '<condition attribute="ownerid" operator="eq-useroruserhierarchy" />';
      }
  
      if (this.operator == 101) {
        return '<condition attribute="ownerid" operator="eq-userteams" />';
      }
  
      var result: string = "";
  
      let _v = this.value;
  
      if (this.value != null) {
        if (typeof this.value == 'number') {
          _v = this.value.toString();
        }
  
        if (typeof this.value == 'boolean') {
          _v = this.value.valueOf() ? 'true' : 'false';
        }
  
        if (this.value != null && this.value.hasOwnProperty('value')) {
          _v = this.value.value;
        }
      }
  
      if (_v == null) {
        this.adjustOperation();
      }
  
      var op = "eq";
      switch (this.operator) {
        case Comparator.Contains: op = "like"; _v = "%" + _v + "%"; break;
        case Comparator.ContainsData: op = "not-null"; break;
        case Comparator.DoesNotContainsData: op = "null"; break;
        case Comparator.EndsWith: op = "like"; _v = "%" + _v; null; break;
        case Comparator.Equals: op = "eq"; break;
        case Comparator.GreaterThan: op = "gt"; break;
        case Comparator.GreaterThanOrEqual: op = "ge"; break;
        case Comparator.LessThan: op = "lt"; break;
        case Comparator.LessThanOrEQual: op = "le"; break;
        case Comparator.NotContains: op = "not-like"; "%" + _v + "%"; break;
        case Comparator.NotEndsWith: op = "not-like"; "&" + _v; break;
        case Comparator.NotEquals: op = "ne"; break;
        case Comparator.NotStartsWith: op = "not-like"; _v = _v + "%"; break;
        case Comparator.StartsWith: op = "like"; _v = _v + "%"; break;
        default: throw "condition type not supported in fetch xml " + this.operator;
      }
  
      var aliasString = "";
      if (this.alias != null) {
        aliasString = " entityname='" + this.alias + "'";
      }
  
      if (_v != null) {
        result += "<condition" + aliasString + " attribute='" + this.field + "' operator='" + op + "' value='" + _v + "' />";
      } else {
        result += "<condition" + aliasString + " attribute='" + this.field + "' operator='" + op + "' />";
      }
      return result;
    }
    private adjustOperation() {
        if (this.operator == Comparator.Equals) {
          this.operator = Comparator.DoesNotContainsData;
        } else
          if (this.operator == Comparator.NotEquals) {
            this.operator = Comparator.ContainsData;
          }
    
        if (this.operator != Comparator.DoesNotContainsData && this.operator != Comparator.ContainsData) {
          throw "null in condition value is only supported for containsdata and doesnotdontainsdata:" + this.field + "/" + (this.alias != null ? "alias:" + this.alias : "") + "/" + this.operator;
        }
      }
    }
    