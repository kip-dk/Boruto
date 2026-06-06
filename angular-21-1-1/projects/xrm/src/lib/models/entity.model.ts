import { EntityReference } from "./entityreference.model";

export class Entity {
    _pluralName: string;
    _keyName: string;
    _updateable: boolean = false;
    _logicalName: string;
    id: string;
    calculatedProperties?: string[];
    constructor(pluralName: string, keyName: string, updateable?: boolean, logicalname?: string, calculatedProperties?: string[]) {
      this._pluralName = pluralName;
      this._keyName = keyName;
      this._updateable = updateable ?? false;
      this._logicalName = "";
      this.id = "";
      this.calculatedProperties = calculatedProperties;
  
      if (logicalname != null && logicalname != '') {
        this._logicalName = logicalname;
      } else {
        let x = this._pluralName != null ? this._pluralName.toLowerCase() : null;
        switch (x) {
          case "emails": this._logicalName = "email"; break;
          case "appointments": this._logicalName = "appointment"; break;
          case "letters": this._logicalName = "letter"; break;
          case "phonecalls": this._logicalName = "phonecall"; break;
          case "tasks": this._logicalName = "task"; break;
          case "campaignresponses": this._logicalName = "campaignresponse"; break;
          default: {
            if (this._keyName.toLowerCase() == "activityid" && x != null) {
              this._logicalName = x.substring(0, x.length - 1);
            } else {
              this._logicalName = this._keyName.substring(0, (this._keyName.length - 2)).toLowerCase();
            }
            break;
          }
        }
      }
    }
  
    ToEntityReference(associatednavigationproperty: string): EntityReference {
      return new EntityReference(this.id ?? "", this._pluralName, associatednavigationproperty, this._logicalName ?? "");
    }
  
    ignoreColumn(prop: string): boolean {
      if (prop == "_pluralName" || prop == "_logicalName" || prop == "_keyName" || prop == "id" || prop == '_updateable' || prop == '$expand' || prop == 'access' || prop == 'calculatedProperties') {
        return true;
      }
      if (this.calculatedProperties && this.calculatedProperties.find(p => p == prop)) {
        return true;
      }
      
      return false;
    }
  
    columns(): string[];
    columns(webapi: boolean): string[];
    columns(webapi: boolean = false): string[] {
      let result = [] as string[];
  
      let columns: string = this._keyName;
      for (var prop in this) {
        if (prop == this._keyName) continue;
        if (this.ignoreColumn(prop)) continue;
        if (this.calculatedProperties?.find(r => prop)) continue;
  
        let v = this[prop];
        
        if (typeof v !== 'undefined' && v != null) {
          if (Array.isArray(v)) {
            continue;
          }
          if (v instanceof Entity) {
            continue;
          }
        }
  
        if (v !== undefined && this.hasOwnProperty(prop)) {
          if (webapi && this[prop] instanceof EntityReference) {
            result.push("_" + prop + "_value");
          } else {
            result.push(prop);
          }
        }
      }
      return result;
    }
  }
  