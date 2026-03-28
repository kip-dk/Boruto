import { Injectable } from "@angular/core";
import { IndexedObject } from "./models/indexedobject.model";
import { XrmContext } from "./models/xrmcontext.interface";
import { XrmContextInstance } from "./models/xrmcontextinstance.model";
import { XrmFormKey } from "./models/xrmformkey.model";
import { XrmRoot } from "./xrmformcontext.interface";

const BorutoXrmLOCAL_getFormType = "BorutoXrmServiceLOCAL_getFormType";
const BorutoXrmLOCAL_formentityr = "BorutoXrmServiceLOCAL_formentityr";
const BorutoXrmLOCAL_context = "BorutoXrmServiceLOCAL_context";

@Injectable({providedIn: 'root'})
export class XrmFormService {

  private _window: IndexedObject = window as IndexedObject;
  constructor() {
  }

  getXrm(): Xrm.XrmStatic | null {
    if (this._window["Xrm"]) return this._window["Xrm"];
    if (window.parent && this._window["parent"]["Xrm"]) return this._window["parent"]["Xrm"];
    if (window.opener && window.opener["Xrm"]) return window.opener["Xrm"];

    return null;
  }

  addXrmHook(name: string, f: () => void, bindto?: any) {
    var xrm = this.getXrm() as any | null;
    if (xrm) {
      if (xrm[name] == undefined) {
        var fun = f;
        if (bindto) {
          fun = () => {
            f.apply(bindto);
          };
        }
        xrm[name] = fun;
      }
    }
  }

  removeXrmHook(name: string) {
    var xrm = this.getXrm() as any | null;
    if (xrm && xrm[name]) {
      delete xrm[name];
    }
  }

  getFormContext(): XrmRoot | null {
    if (this._window["Xrm"]) return this._window["Xrm"];
    if (window.parent && this._window["parent"]["Xrm"]) return this._window["parent"]["Xrm"];
    if (window.opener && window.opener["Xrm"]) return window.opener["Xrm"];
    return null;
  }

  getContext(): XrmContext | null {
    if (this._window[BorutoXrmLOCAL_context]) {
      return this._window[BorutoXrmLOCAL_context];
    }

    if (typeof this._window["GetGlobalContext"] != "undefined") {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(this._window["GetGlobalContext"]());
      return this._window[BorutoXrmLOCAL_context];
    }

    if (window.parent && this._window["parent"][BorutoXrmLOCAL_context]) {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(this._window["parent"][BorutoXrmLOCAL_context]);
      return this._window[BorutoXrmLOCAL_context];
    }

    if (window.opener && window.opener[BorutoXrmLOCAL_context]) {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(window.opener[BorutoXrmLOCAL_context]);
      return this._window[BorutoXrmLOCAL_context];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Utility"] && this._window["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(this._window["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[BorutoXrmLOCAL_context];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Utility"] && this._window["parent"]["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(this._window["parent"]["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[BorutoXrmLOCAL_context];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Utility"] && window.opener["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[BorutoXrmLOCAL_context] = new XrmContextInstance(window.opener["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[BorutoXrmLOCAL_context];
    }

    return null;
  }

  // "Xrm", "Page", "ui", "getFormType"


  getFormType(): number | null;
  getFormType(clear?: boolean): number | null {

    if (!clear && this._window[BorutoXrmLOCAL_getFormType]) delete this._window[BorutoXrmLOCAL_getFormType];
    if (this._window[BorutoXrmLOCAL_getFormType]) {
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    if (!clear && window.parent && this._window["parent"][BorutoXrmLOCAL_getFormType]) {
      this._window[BorutoXrmLOCAL_getFormType] = this._window["parent"][BorutoXrmLOCAL_getFormType];
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    if (!clear && window.opener && window.opener[BorutoXrmLOCAL_getFormType]) {
      this._window[BorutoXrmLOCAL_getFormType] = window.opener[BorutoXrmLOCAL_getFormType];
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Page"] && this._window["Xrm"]["Page"]["ui"] && this._window["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[BorutoXrmLOCAL_getFormType] = this._window["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Page"] && this._window["parent"]["Xrm"]["Page"]["ui"] && this._window["parent"]["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[BorutoXrmLOCAL_getFormType] = this._window["parent"]["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Page"] && window.opener["Xrm"]["Page"]["ui"] && window.opener["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[BorutoXrmLOCAL_getFormType] = window.opener["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[BorutoXrmLOCAL_getFormType];
    }

    return null;
  }

  // "Xrm", "Page", "data", "entity"
  getFormKey(id: string, type: string): XrmFormKey;
  getFormKey(id: string, type: string, clear: boolean): XrmFormKey;
  getFormKey(id: string, type: string, clear: boolean = false): XrmFormKey {
    if (typeof id != 'undefined' && id != null && id != '' && typeof type != 'undefined' && type != null && type != '') {
      let result = new XrmFormKey();
      result.id = id;
      result.type = type;
      this._window[BorutoXrmLOCAL_formentityr] = result;
      return result;
    }

    if (clear && this._window[BorutoXrmLOCAL_formentityr]) delete this._window[BorutoXrmLOCAL_formentityr];
    if (this._window[BorutoXrmLOCAL_formentityr]) {
      return this._window[BorutoXrmLOCAL_formentityr];
    }


    if (!clear && window.parent && this._window["parent"][BorutoXrmLOCAL_formentityr]) {
      this._window[BorutoXrmLOCAL_formentityr] = this._window["parent"][BorutoXrmLOCAL_formentityr];
      return this._window[BorutoXrmLOCAL_formentityr];
    }

    if (!clear && window.opener && window.opener[BorutoXrmLOCAL_formentityr]) {
      this._window[BorutoXrmLOCAL_formentityr] = window.opener[BorutoXrmLOCAL_formentityr];
      return this._window[BorutoXrmLOCAL_formentityr];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Page"] && this._window["Xrm"]["Page"]["data"] && this._window["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = this._window["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = this._window["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();
      this._window[BorutoXrmLOCAL_formentityr] = result;
      return this._window[BorutoXrmLOCAL_formentityr];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Page"] && this._window["parent"]["Xrm"]["Page"]["data"] && this._window["parent"]["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = this._window["parent"]["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = this._window["parent"]["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();
      this._window[BorutoXrmLOCAL_formentityr] = result;
      return this._window[BorutoXrmLOCAL_formentityr];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Page"] && window.opener["Xrm"]["Page"]["data"] && window.opener["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = window.opener["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = window.opener["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();

      this._window[BorutoXrmLOCAL_formentityr] = result;
      return this._window[BorutoXrmLOCAL_formentityr];
    }
    return new XrmFormKey();
  }
}
