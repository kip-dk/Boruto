import { Entity } from "./entity.model";
import { LabelMeta } from "./labelmeta.model";
import { LookupAttribute } from "./lookupattribute.interface";

export class AttributeMeta extends Entity {
    constructor() {
      super("Attributes", "MetadataId")
    }
    AttributeType?: string;
    DisplayName?: LabelMeta;
    LogicalName: string = "";
    Description?: LabelMeta;
    SchemaName: string = "";
  
    // virtual properties for ui purpose
    selected?: boolean;
    Lookup?: LookupAttribute;
  
    onFetch() {
      this.selected = false;
    }
  }
  