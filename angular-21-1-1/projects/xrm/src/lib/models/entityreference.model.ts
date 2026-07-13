import { effect, signal } from "@angular/core";

export class EntityReference {

  static ToEntityReference(a: any, f: string): EntityReference {
    var result = new EntityReference();

    if (a[f] == undefined) {
      return new EntityReference();
    }

    result.id = a[f];
    result.logicalname = a[f+"@Microsoft.Dynamics.CRM.lookuplogicalname"];
    result.name = a[f+"@OData.Community.Display.V1.FormattedValue"];

    delete a[f];
    delete a[f+"@Microsoft.Dynamics.CRM.lookuplogicalname"];
    delete a[f+"@OData.Community.Display.V1.FormattedValue"];
    delete a[f+"@OData.Community.Display.V1.AttributeName"]

    return result;
  }

    id: string;
    name: string;
    logicalname: string;
    associatednavigationproperty: string;
    pluralName: string;

    id$ = signal<string | undefined>(undefined);
    name$ = signal<string | undefined>(undefined);

    constructor(id?: string, pluralName?: string, associatednavigationproperty?: string, logicalname?: string) {
      this.id = id ?? "";
      this.pluralName = pluralName ?? "";
      this.associatednavigationproperty = associatednavigationproperty ?? "";
      this.logicalname = logicalname ?? "";
      this.name = "";
  
      if (this.pluralName != "" && logicalname == "") {
        switch (this.pluralName.toLowerCase()) {
          case "emails": this.logicalname = "email"; break;
          case "appointments": this.logicalname = "appointment"; break;
          case "letters": this.logicalname = "letter"; break;
          case "phonecalls": this.logicalname = "phonecall"; break;
          case "tasks": this.logicalname = "task"; break;
          default: {
            this.logicalname = this.pluralName.substring(0, (this.pluralName.length - 1)).toLowerCase();
            break;
          }
        }
      }

      this.id$.set(this.id);
      this.name$.set(this.name);

      effect(() => this.id = this.id$() ?? '');
      effect(() => this.name = this.name$() ?? '');
    }
  
    replace(v1: string, v2: string) {
      this.id.replace(v1, v2);
    }
  
    meta(pluralName: string, associatednavigationproperty: string): EntityReference {
      this.pluralName = pluralName;
      this.associatednavigationproperty = associatednavigationproperty;
      return this;
    }
  
    clone(): EntityReference {
      let result = new EntityReference();
      result.id = this.id;
      result.name = this.name;
      result.logicalname = this.name;
      result.associatednavigationproperty = this.associatednavigationproperty;
      return result;
    }
  
    associatednavigationpropertyname(): string {
      if (this.associatednavigationproperty == null || this.associatednavigationproperty == '') {
        throw 'navigation property has not been set for this EntityReference instance';
      }
  
      if (this.associatednavigationproperty.endsWith('@odata.bind')) {
        return this.associatednavigationproperty;
      }
      return this.associatednavigationproperty + '@odata.bind';
    }
  
    equals(ref: EntityReference): boolean {
      return this.id == ref.id && this.logicalname == ref.logicalname;
    }
  
    toJsonProperty(): any {
      return { '@odata.id': "'" + this.pluralName + "(" + this.id.replace("{", "").replace("}", "") + ")" + "'" };
    }
  
    static same(ref1: EntityReference, ref2: EntityReference): boolean {
      if (ref1 == null && ref2 == null) {
        return true;
      }
  
      let id1: string | null = null;
      let id2: string | null = null;
      if (ref1 != null) id1 = ref1.id;
      if (ref2 != null) id2 = ref2.id;
      return id1 == id2;
    }
  }