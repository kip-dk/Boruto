import { Entity } from "./entity.model";

export interface ExpandProperty {
    name: string;
    entity: Entity;
    isArray: boolean;
    value?: any;
  }
  