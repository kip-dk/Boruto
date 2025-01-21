import { Observable, map } from "rxjs";
import { XrmService } from "../xrm.service";
import { Entity } from "./entity.model";

export class Entities<T extends Entity> extends Array<T> {
    constructor(fType: string, tType: string, refName: string, leftToRight: boolean, t: T) {
      super(0);
      Object.setPrototypeOf(this, new.target.prototype);
      this.parentType = fType;
      this.childType = tType;
      this.refName = refName;
      this.leftToRight = leftToRight;
      this.parentId = "";
      this.push(t);
    }
  
    private parentType: string;
    private parentId: string;
    private childType: string;
    private refName: string;
    private leftToRight: boolean;
    private xrmService?: XrmService;
  
    add(entity: Entity): Observable<null> {
      let fromType = this.parentType;
      let fromId = this.parentId;
      let toType = this.childType;
      let toId = entity.id;
  
      if (!this.leftToRight) {
        fromType = this.childType;
        fromId = entity.id;
        toType = this.parentType;
        toId = this.parentId;
      }
  
      if (this.xrmService != null) { 
        return this.xrmService.associate(fromType, fromId as string, toType, toId as string, this.refName).pipe(map(r => {
          this.push(entity as T);
          return null;
      }));
      }
      throw new Error("xrmService is null");
    }
  
    remove(entity: Entity): Observable<null> {
      let fromType = this.parentType;
      let fromId = this.parentId;
      let toType = this.childType;
      let toId = entity.id;
  
      if (!this.leftToRight) {
        fromType = this.childType;
        fromId = entity.id;
        toType = this.parentType;
        toId = this.parentId;
      }
  
      if (this.xrmService != null) {
        return this.xrmService.disassociate(fromType, fromId, toType, toId, this.refName).pipe(map(r => {
          var index = this.indexOf(entity as T);
          if (index >= 0) {
            this.splice(index, 1);
          }
          return null;
        }));
      }
      throw new Error("xrmService is null");
    }
  }
  