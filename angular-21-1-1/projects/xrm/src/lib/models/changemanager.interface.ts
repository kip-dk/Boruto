import { Entity } from "./entity.model";

export interface ChangeManager {
    hasChanges(prototype: Entity, instance: Entity): boolean;
}