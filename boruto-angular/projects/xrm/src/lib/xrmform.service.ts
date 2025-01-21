import { Injectable } from "@angular/core";
import { IndexedObject } from "./models/indexedobject.model";
import { XrmContext } from "./models/xrmcontext.interface";
import { XrmContextInstance } from "./models/xrmcontextinstance.model";
import { XrmFormKey } from "./models/xrmformkey.model";
import { XrmFormContext } from "./xrmformcontext.interface";

const HoforXrmLOCAL_getFormType = "HoforXrmServiceLOCAL_getFormType";
const HoforXrmLOCAL_formentityr = "HoforXrmServiceLOCAL_formentityr";
const HoforXrmLOCAL_context = "HoforXrmServiceLOCAL_context";

@Injectable()
export class XrmFormService {

  private _window: IndexedObject = window as IndexedObject;
  constructor() {
  }

  getFormContext(): XrmFormContext | null{
    if (this._window["Xrm"]) return this._window["Xrm"];
    if (window.parent && this._window["parent"]["Xrm"]) return this._window["parent"]["Xrm"];
    if (window.opener && window.opener["Xrm"]) return window.opener["Xrm"];
    return null;
  }

  getContext(): XrmContext | null {
    if (this._window[HoforXrmLOCAL_context]) {
      return this._window[HoforXrmLOCAL_context];
    }

    if (typeof this._window["GetGlobalContext"] != "undefined") {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(this._window["GetGlobalContext"]());
      return this._window[HoforXrmLOCAL_context];
    }

    if (window.parent && this._window["parent"][HoforXrmLOCAL_context]) {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(this._window["parent"][HoforXrmLOCAL_context]);
      return this._window[HoforXrmLOCAL_context];
    }

    if (window.opener && window.opener[HoforXrmLOCAL_context]) {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(window.opener[HoforXrmLOCAL_context]);
      return this._window[HoforXrmLOCAL_context];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Utility"] && this._window["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(this._window["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[HoforXrmLOCAL_context];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Utility"] && this._window["parent"]["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(this._window["parent"]["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[HoforXrmLOCAL_context];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Utility"] && window.opener["Xrm"]["Utility"]["getGlobalContext"]) {
      this._window[HoforXrmLOCAL_context] = new XrmContextInstance(window.opener["Xrm"]["Utility"]["getGlobalContext"]());
      return this._window[HoforXrmLOCAL_context];
    }

    return null;
  }

  // "Xrm", "Page", "ui", "getFormType"


  getFormType(): number | null;
  getFormType(clear?: boolean): number | null {

    if (!clear && this._window[HoforXrmLOCAL_getFormType]) delete this._window[HoforXrmLOCAL_getFormType];
    if (this._window[HoforXrmLOCAL_getFormType]) {
      return this._window[HoforXrmLOCAL_getFormType];
    }

    if (!clear && window.parent && this._window["parent"][HoforXrmLOCAL_getFormType]) {
      this._window[HoforXrmLOCAL_getFormType] = this._window["parent"][HoforXrmLOCAL_getFormType];
      return this._window[HoforXrmLOCAL_getFormType];
    }

    if (!clear && window.opener && window.opener[HoforXrmLOCAL_getFormType]) {
      this._window[HoforXrmLOCAL_getFormType] = window.opener[HoforXrmLOCAL_getFormType];
      return this._window[HoforXrmLOCAL_getFormType];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Page"] && this._window["Xrm"]["Page"]["ui"] && this._window["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[HoforXrmLOCAL_getFormType] = this._window["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[HoforXrmLOCAL_getFormType];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Page"] && this._window["parent"]["Xrm"]["Page"]["ui"] && this._window["parent"]["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[HoforXrmLOCAL_getFormType] = this._window["parent"]["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[HoforXrmLOCAL_getFormType];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Page"] && window.opener["Xrm"]["Page"]["ui"] && window.opener["Xrm"]["Page"]["ui"]["getFormType"]) {
      this._window[HoforXrmLOCAL_getFormType] = window.opener["Xrm"]["Page"]["ui"]["getFormType"]();
      return this._window[HoforXrmLOCAL_getFormType];
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
      this._window[HoforXrmLOCAL_formentityr] = result;
      return result;
    }

    if (clear && this._window[HoforXrmLOCAL_formentityr]) delete this._window[HoforXrmLOCAL_formentityr];
    if (this._window[HoforXrmLOCAL_formentityr]) {
      return this._window[HoforXrmLOCAL_formentityr];
    }


    if (!clear && window.parent && this._window["parent"][HoforXrmLOCAL_formentityr]) {
      this._window[HoforXrmLOCAL_formentityr] = this._window["parent"][HoforXrmLOCAL_formentityr];
      return this._window[HoforXrmLOCAL_formentityr];
    }

    if (!clear && window.opener && window.opener[HoforXrmLOCAL_formentityr]) {
      this._window[HoforXrmLOCAL_formentityr] = window.opener[HoforXrmLOCAL_formentityr];
      return this._window[HoforXrmLOCAL_formentityr];
    }

    if (this._window["Xrm"] && this._window["Xrm"]["Page"] && this._window["Xrm"]["Page"]["data"] && this._window["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = this._window["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = this._window["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();
      this._window[HoforXrmLOCAL_formentityr] = result;
      return this._window[HoforXrmLOCAL_formentityr];
    }

    if (window.parent && this._window["parent"]["Xrm"] && this._window["parent"]["Xrm"]["Page"] && this._window["parent"]["Xrm"]["Page"]["data"] && this._window["parent"]["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = this._window["parent"]["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = this._window["parent"]["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();
      this._window[HoforXrmLOCAL_formentityr] = result;
      return this._window[HoforXrmLOCAL_formentityr];
    }

    if (window.opener && window.opener["Xrm"] && window.opener["Xrm"]["Page"] && window.opener["Xrm"]["Page"]["data"] && window.opener["Xrm"]["Page"]["data"]["entity"]) {
      var result = new XrmFormKey();
      result.id = window.opener["Xrm"]["Page"]["data"]["entity"]["getId"]();
      result.type = window.opener["Xrm"]["Page"]["data"]["entity"]["getEntityName"]();

      this._window[HoforXrmLOCAL_formentityr] = result;
      return this._window[HoforXrmLOCAL_formentityr];
    }
    return new XrmFormKey();
  }
}
