export interface OneToManyRelationship {
    MetadataId: string;
    RelationshipType: number;
    SchemaName: string;
    ReferencedAttribute: string;
    ReferencingAttribute: string;
    ReferencedEntity: string;
    ReferencingEntity: string;
    ReferencingEntityNavigationPropertyName: string;
  }
  