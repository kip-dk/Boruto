import { AttributeMeta } from "./attributemeta.model";
import { Entity } from "./entity.model";
import { LabelMeta } from "./labelmeta.model";
import { ManyToManyRelationship } from "./manytomanyrelationship.interface";
import { OneToManyRelationship } from "./onetomanyrelationship.interface";

export class EntityMeta extends Entity {
    constructor() {
      super("EntityDefinitions", "MetadataId");
    }
  
    DisplayName?: LabelMeta;
    LogicalName: string = "";
    ObjectTypeCode: number = 0;
    SchemaName: string = "";
    LogicalCollectionName: string = "";
    IsActivity: boolean = false;
    IsActivityParty: boolean = false;
    Attributes?: AttributeMeta[];
  
    OneToManyRelations?: OneToManyRelationship[];
    ManyToManyRelations?: ManyToManyRelationship[];
  
    meta(): EntityMeta {
      this.Attributes = [new AttributeMeta()];
      return this;
    }
  }
  