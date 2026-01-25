import { Comparator } from "./comparator.enum";
import { Entity } from "./entity.model";
import { Filter } from "./filter.model";
import { Operator } from "./operator.enum";

export class Condition {
    operator: Operator = Operator.And;
    filter: Filter[];
    children: Condition[];
    parent?: Condition;
  
    constructor();
    constructor(operator: Operator);
    constructor(operator: Operator = Operator.And) {
      this.operator = operator;
      this.filter = [];
      this.children = [];
    }
  
    where(field: string, opr: Comparator): Condition;
    where(field: string, opr: Comparator, value: any): Condition;
    where(field: string, opr: Comparator, value: any = null): Condition {
      let f = new Filter();
      f.field = field;
      f.value = value;
      f.operator = opr;
      this.filter.push(f);
      return this;
    }
  
    exact(field: string, value: any = null): Condition {
      let f = new Filter();
      f.field = field;
      if (value == null || value == undefined || value == '') {
        f.operator = Comparator.DoesNotContainsData;
      } else {
        f.operator = Comparator.Equals;
        f.value = value;
      }
      this.filter.push(f);
      return this;
    }

    alias(alias: string, field: string, opr: Comparator): Condition;
    alias(alias: string, field: string, opr: Comparator, value: any): Condition;
    alias(alias: string, field: string, opr: Comparator, value: any = null): Condition {
      let f = new Filter();
      f.alias = alias;
      f.field = field;
      f.value = value;
      f.operator = opr;
      this.filter.push(f);
      return this;
    }
  
    group(opr: Operator): Condition {
      let result: Condition = new Condition(opr);
      result.parent = this;
  
      this.children.push(result);
      return result;
    }
  
    isActive(): Condition {
      return this.where("statecode", Comparator.Equals, 0);
    }

    containsAllWords(search: string, fields: string[]) {
      var split = search.split(' ').filter(r => r != '');
      if (split.length > 0 && fields.length > 0) {
        split.forEach(word => {
          var next = this.group(Operator.And);
          var sub = next.group(Operator.Or);
          fields.forEach(field => {
            sub.where(field, Comparator.Contains, word);
          });
        });
      }
    }

    fieldMatchAllWords(search: string, fields: string[]) {
      if (search != null && search.length > 0 && fields != null && fields.length > 0) {
        var words = search.split(' ');
        var top = this.group(Operator.Or);

        fields.forEach(field => {
          var ff = top.group(Operator.And);
          words.forEach(word => {
            ff.where(field, Comparator.Contains, word);
          });
        });
      }
    }
  
    isInactive(): Condition {
      return this.where("statecode", Comparator.Equals, 1);
    }
  
    owningUserIsCurrentUserOrHirachy(): Condition {
      return this.where("ownerid", 100)
    }
  
    currentUserIsMemberOfOwningTeam(): Condition {
      return this.where("ownerid", 101)
    }
  
  
    raw(filter: string): Condition {
      let result = new Filter();
      result.field = filter;
      result["raw"] = true;
      this.filter.push(result);
      return this;
    }
  
    toQueryString(prototype: Entity): string | null {
      if ((this.children == null || this.children.length == 0) && (this.filter == null || this.filter.length == 0)) {
        return null;
      }
  
      let me = this;
      let result = '';
      let opr = '';
      if (this.filter != null && this.filter.length > 0) {
        this.filter.forEach(r => {
          result += opr + r.toQueryString(prototype);
          if (me.operator == Operator.And) {
            opr = ' and ';
          } else {
            opr = ' or ';
          }
  
        });
      }
  
      if (this.children != null && this.children.length > 0) {
        this.children.forEach(c => {
          var tmpstring = "(" + c.toQueryString(prototype) + ")";

          if (tmpstring != '' && tmpstring != '()' && tmpstring != '(null)') {
            result += opr + tmpstring;
          }


          if (me.operator == Operator.And) {
            opr = ' and ';
          } else {
            opr = ' or ';
          }
        });
      }
      return result;
    }
  
    toFetchXml(): string {
      let result: string = "";
  
      result += "<filter type='" + (this.operator == Operator.And ? "and" : "or") + "'>";
      if (this.filter != null && this.filter.length > 0)
        this.filter.forEach(f => {
          result += f.toFetcmXml();
        });
  
      if (this.children != null && this.children.length > 0) {
        this.children.forEach(c => {
          result += c.toFetchXml();
        });
      }
  
      result += "</filter>"
  
      return result;
    }
  }
  