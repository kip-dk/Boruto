import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { XrmService } from "./xrm.service";
import { XrmContext } from "./models/xrmcontext.interface";
import { XrmEntityKey } from "./models/xrmentitykey.model";
import { Observable, map, lastValueFrom, throwError, BehaviorSubject } from "rxjs";
import { Entity } from "./models/entity.model";
import { Expand } from "./models/expand.model";
import { Fetchxml } from "./models/fetchxml.model";
import { XrmQueryResult } from "./models/xrmqueryresult.interface";
import { Condition } from "./models/condition.model";
import { XrmTransaction } from "./models/xrmtransaction.model";
import { IndexedObject } from "./models/indexedobject.model";
import { XrmTransactionItem } from "./models/xrmtransactionitem.model";
import { EntityReference } from "./models/entityreference.model";
import { OptionSetValue } from "./models/optionsetvalue.model";
import { XrmAccess } from "./models/xrmaccess.model";
import { ExpandProperty } from "./models/expandproperty.model";
import { Entities } from "./models/entities.model";
import { ColumnBuilder } from "./models/columnbuilder.model";


const XRMCONTEXTSERVICE_EMPTY_GUID = "00000000-0000-0000-0000-000000000000";
const isNum = (num:any) => num != null && typeof num !== 'object' && (!Number.isNaN(+(String((String(num) || '').replace(/[^0-9\.\-e]/, '') !== String(num) || num === '' ? NaN : num))));

@Injectable({providedIn: 'root'})
export class XrmContextService {
  private context: Map<string, Entity> = new Map<string, Entity>();
  private changemanager: any = {};
  private tick: number = new Date().valueOf();
  private includeOriginalPayload$: boolean = false;

  pageCookieMatch = /[-pagingcookie=\"][a-z,A-Z,0-9,%-_.~]+/g;

  constructor(private http: HttpClient, private xrmService: XrmService) { }

  setVersion(v: string) {
    this.xrmService.setVersion(v);
  }

  includeOroginalPayload(v: boolean): void {
    this.includeOriginalPayload$ = v;
  }




  getContext(): XrmContext {
    return this.xrmService.getContext();
  }

  getCurrentKey(): Observable<XrmEntityKey>;
  getCurrentKey(repeatForCreateForm: boolean): Observable<XrmEntityKey>;
  getCurrentKey(repeatForCreateForm: boolean = false): Observable<XrmEntityKey> {
    return this.xrmService.getCurrentKey(repeatForCreateForm);
  }

  getServiceUrl(): string {
    return this.getContext().getClientUrl() + this.xrmService.apiUrl;
  }

  getCurrentUserId(): BehaviorSubject<string> {
    return this.xrmService.getCurrentUserId();
  }

  get<T extends Entity>(prototype: T, id: string): Observable<T> {
    let me = this;
    let columnDef = this.columnBuilder(prototype);

    let expand: Expand | null = null;

    let eps = this.getExpandProperties(prototype);
    if (eps != null && eps.length > 0) {
      let comma = ",";
      eps.forEach(ep => {
        if (expand == null) {
          expand = this.$expandToExpand(ep);
        } else {
          if (expand.additional == null) {
            expand.additional = [];
          }

          var next = this.$expandToExpand(ep);
          if (next != null) {
            expand.additional.push(next);
          }
        }
      });
    }

    if (expand != null) {
      return this.xrmService.get<T>(prototype._pluralName, id, columnDef.columns, expand).pipe(map(r => {
        return me.resolve<T>(prototype, r, prototype._updateable);
      }));
    } else {
      return this.xrmService.get<T>(prototype._pluralName, id, columnDef.columns).pipe(map(r => {
        return me.resolve<T>(prototype, r, prototype._updateable);
      }));
    }
  }

  debug(setting: boolean): void {
    this.xrmService.debug = setting;
  }

  count(xml: Fetchxml): Observable<number> {
    let me = this;
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    headers = headers.append("Cache-Control", "no-cache");

    let options = {
      headers: headers
    }

    let fetchxml = xml.toCountFetchXml();
    let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + xml.entity().entityPrototype._pluralName + "?fetchXml=" + encodeURIComponent(fetchxml);

    this.xrmService.log(fetchxml);
    this.xrmService.log(url);

    return new Observable(obs => {
      lastValueFrom(this.http.get<any>(this.forceHTTPS(url), options))
        .then(response => {
          if (response.value && response.value.length && response.value.length == 1) {
            obs.next(response.value[0].count);
          } else {
            obs.next(0);
          }
        })
        .catch(e => {
          if (e["error"] && e["error"]["error"] && e["error"]["error"]["code"]) {
            var val = e["error"]["error"]["code"];
            if (val != null && val.toString() == "0x8004e023") {
              obs.next(50000);
              return;
            }
          }
          throwError(() => new Error(e));
        });
    });
  }

  fetch<T extends Entity>(xml: Fetchxml): Observable<XrmQueryResult<T>> {
    let me = this;
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    headers = headers.append("Cache-Control", "no-cache");

    let options = {
      headers: headers
    }

    let fetchxml = xml.toFetchXml();
    let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + xml.entity().entityPrototype._pluralName + "?fetchXml=" + encodeURIComponent(fetchxml);

    this.xrmService.log(fetchxml);
    this.xrmService.log(url);

    let localPrototype = xml.entity().entityPrototype as T;

    return this.http.get(this.forceHTTPS(url), options).pipe(map(response => {
      let result = me.resolveFetchResult<T>(localPrototype, response, xml.count, [url], 0, xml);
      return result;
    }));
  }

  fetchxml<T extends Entity>(prototype: T, fetchxml: string): Observable<XrmQueryResult<T>> {
    let me = this;
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    headers = headers.append("Cache-Control", "no-cache");

    let options = {
      headers: headers
    }

    let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + prototype._pluralName + "?fetchXml=" + encodeURIComponent(fetchxml);

    this.xrmService.log(fetchxml);
    this.xrmService.log(url);

    return this.http.get(this.forceHTTPS(url), options).pipe(map(response => {
      let result = me.resolveQueryResult<T>(prototype, response, 0, [url], 0);
      return result;
    }));
  }

  query<T extends Entity>(prototype: T, condition: Condition, orderBy?: string, top?: number, count?: boolean): Observable<XrmQueryResult<T>> {
    let me = this;
    let fields = this.columnBuilder(prototype).columns;

    let con = condition;
    let filter = null;
    if (condition != null) {
      while (con.parent != null) { con = con.parent };

      filter = con.toQueryString(prototype);
    }

    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    if (top != null && top > 0) {
      headers = headers.append("Prefer", "odata.maxpagesize=" + top.toString());
    }
    headers = headers.append("Cache-Control", "no-cache");

    let options = {
      headers: headers
    }

    let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + prototype._pluralName;
    if ((fields != null && fields != '') || (filter != null && filter != '') || (orderBy != null && orderBy != '') || (top != null && top > 0)) {
      url += "?";
    }
    let sep = '';

    if (fields != null && fields != '') {
      url += '$select=' + fields;
      sep = '&';
    }

    if (filter != null && filter != '') {
      url += sep + '$filter=' + filter;
      sep = '&';
    }

    if (orderBy != null && orderBy != '') {
      url += sep + '$orderby=' + orderBy;
      sep = '&';
    }

    if (count) {
      url += sep + '$count=true';
      sep = '&';
    }

    this.xrmService.log(url);

    return this.http.get(this.forceHTTPS(url), options).pipe(map(response => {
      let result = me.resolveQueryResult<T>(prototype, response, top ?? 0, [url], 0);
      return result;
    }));
  }

  create<T extends Entity>(prototype: T, instance: T): Observable<T> {
    let newr = this.prepareNewInstance(prototype, instance);
    this.xrmService.log(newr);

    return this.xrmService.create<T>(prototype._pluralName, newr as T).pipe(map(_response => {
      let response = _response as T;
      this.xrmService.log(response);

      if (response != null) {
        if (response.hasOwnProperty('$keyonly')) {
          instance._pluralName = prototype._pluralName;
          instance._logicalName = prototype._logicalName;
          instance._keyName = prototype._keyName;
          instance.id = _response.id;
          instance._updateable = true;

          let key = response._pluralName + ':' + response.id;
          for (let prop in prototype) {
            if (typeof prototype[prop] === 'function') {
              response[prop] = prototype[prop];
              continue;
            }
          }

          this.context.set(key, instance);
          this.updateCM(prototype, instance);

          return instance;
        } else {
          this.resolveNewInstance(prototype, instance, response);
          return this.resolve(prototype, response, true, []);
        }
      }

      throw new Error("response was null")
    }));
  }

  createAll<T extends Entity>(prototype: T, instances: T[]): Observable<null> {
    if (prototype != null && instances != null && instances.length > 0) {
      let trans = new XrmTransaction();
      instances.forEach(r => {
        trans.create(prototype, r);
      });

      return this.commit(trans);
    }
    throw "You must parse a prototype and at least one instance to be created";
  }

  update<T extends Entity>(prototype: T, instance: T): Observable<T>
  update<T extends Entity>(prototype: T, instance: T, deleteReferenceAsEmptyGuid: boolean): Observable<T>
  update<T extends Entity>(prototype: T, instance: T, deleteReferenceAsEmptyGuid: boolean = false): Observable<T> {
    let me = this;
    let upd = this.prepareUpdate(prototype, instance, deleteReferenceAsEmptyGuid);

    if (upd == null) {
      upd = {};
    }

    this.xrmService.log(upd);

    let fields = this.columnBuilder(prototype).columns;

    return this.xrmService.update<T>(prototype._pluralName, upd as T, instance.id, fields).pipe(map(response => {

      var re = response as IndexedObject;
      var wasnull = re == null || re[prototype._keyName] == null || re[prototype._keyName] == '';

      if (wasnull || this.getContext().getVersion().startsWith("8.0") || this.getContext().getVersion().startsWith("8.1")) {
        this.xrmService.log('version 8.0 update');
        this.updateCM(prototype, instance);
        return instance;
      }

      this.xrmService.log('version 9.2 or higher update');
      return me.resolve(prototype, response, true);
    }));
  }

  put(prototype: Entity, instance: Entity, field: string, value?: any): Observable<null> {
    if (value == null) {
      value = 'ko-@value-not-parsed!';
    }

    let v = value;
    if (value == 'ko-@value-not-parsed!') {
      var iv = instance as IndexedObject;
      v = iv[field];
    }

    let pvx = this.preparePutValue(prototype, field, v);
    return this.xrmService.put(prototype._pluralName, instance.id, pvx.field, pvx.value).pipe(map(response => {
      if (value != 'ko-@value-not-parsed!') {
        this.assignValue(prototype, instance, field, value);
      }
      return null;
    }));
  }

  putAll(prototype: Entity, instances: Entity[], field: string, value: any): Observable<null> {
    if (instances != null && instances.length == 1) {
      return this.put(prototype, instances[0], field, value);
    }

    if (instances != null && instances.length > 1) {
      let trans = new XrmTransaction();

      let pvx = this.preparePutValue(prototype, field, value);

      instances.forEach(r => {
        trans.put(prototype, r, field, pvx);
      });
      return this.commit(trans);
    }

    throw 'you must parse at least one instance in the instances array';
  }

  delete<T extends Entity>(t: T): Observable<null> {
    let me = this;
    return this.xrmService.delete(t._pluralName, t.id).pipe(map(r => {
      let key = t._pluralName + ":" + t.id;
      if (me.context.hasOwnProperty(key)) {
        me.context.delete(key);
      }
      return null;
    }));
  }

  deleteAll<T extends Entity>(instances: T[]): Observable<null> {
    if (instances != null && instances.length == 1) {
      return this.delete(instances[0]);
    }

    if (instances != null && instances.length > 0) {
      let trans = new XrmTransaction();
      instances.forEach(r => {
        trans.delete(r);
      });
      return this.commit(trans);
    }
    throw 'you must parse at least one instance in the instances array';
  }

  markChangesCommitted(prototype: Entity, instance: Entity): void {
    this.updateCM(prototype, instance);
  }

  /* marked private becase the api is not well tested yet, to avoi */
  func(name: string, data: any, entity?: Entity): Observable<any> {
    var parameters = data;
    if (parameters != null) {
      parameters = this.toFuncParameterString(data);
    }

    if (entity == null) {
      return this.xrmService.func(name, parameters);
    } else {
      return this.xrmService.func(name, parameters, entity._pluralName, entity.id);
    }
  }


  action(name: string, data: any, entity?: Entity): Observable<any> {
    if (entity != null && entity._pluralName != null && entity.id != null) {
      return this.xrmService.action(name, data, entity._pluralName, entity.id);
    } else {
      return this.xrmService.action(name, data);
    }
  }

  commit(transaction: XrmTransaction): Observable<null> {
    let oprs = transaction["oprs"] as XrmTransactionItem[];

    if (oprs != null && oprs.length > 0) {
      this.tick++;
      let batch = 'batch_KO' + new Date().valueOf() + "$" + this.tick.toString();
      let change = 'changeset_KO' + new Date().valueOf() + "!" + this.tick.toString();
      let headers = new HttpHeaders({ 'Accept': 'application/json' });
      headers = headers.append("Content-Type", "multipart/mixed;boundary=" + batch);
      headers = headers.append("OData-MaxVersion", "4.0");
      headers = headers.append("OData-Version", "4.0");

      let body = '--' + batch + "\n";
      body += "Content-Type: multipart/mixed;boundary=" + change + "\n";
      body += "\n";

      let count = 1;
      oprs.forEach(r => {
        r.id = count;
        if (r.type == "put" && r.field != null) {
          let nextV = this.preparePutValue(r.prototype, r.field, r.value);
          body += '--' + change + "\n";
          body += "Content-Type: application/http\n";
          body += "Content-Transfer-Encoding:binary\n";
          body += "Content-ID: " + count.toString() + "\n";
          body += "\n";
          if (nextV.value != null) {
            body += "PUT " + this.getContext().$devClientUrl() + r.prototype._pluralName + "(" + r.instance.id + ")/" + nextV.field + " HTTP/1.1\n";
          } else {
            body += "DELETE " + this.getContext().$devClientUrl() + r.instance._pluralName + "(" + r.instance.id + ")/" + nextV.field + " HTTP/1.1\n";
          }

          let xr = {} as IndexedObject;
          if (nextV.value != null) {
            xr[nextV.propertyAs] = nextV.value;
          }
          body += "Content-Type: application/json;type=entry\n";
          body += "\n";
          body += JSON.stringify(xr) + "\n";
        }

        if (r.type == "delete") {
          body += '--' + change + "\n";
          body += "Content-Type: application/http\n";
          body += "Content-Transfer-Encoding:binary\n";
          body += "Content-ID: " + count.toString() + "\n";
          body += "\n";
          body += "DELETE " + this.getContext().$devClientUrl() + r.instance._pluralName + "(" + r.instance.id + ")" + " HTTP/1.1\n";
          body += "Content-Type: application/json;type=entry\n";
          body += "\n";
          body += "{ }\n";
        }

        if (r.type == "create") {
          let nextI = this.prepareNewInstance(r.prototype, r.instance);
          body += '--' + change + "\n";
          body += "Content-Type: application/http\n";
          body += "Content-Transfer-Encoding:binary\n";
          body += "Content-ID: " + count.toString() + "\n";
          body += "\n";
          body += "POST " + this.getContext().$devClientUrl() + r.prototype._pluralName + " HTTP/1.1\n";
          body += "Content-Type: application/json;type=entry\n";
          body += "\n";
          body += JSON.stringify(nextI) + "\n";
        }

        if (r.type == "update") {
          let nextU = this.prepareUpdate(r.prototype, r.instance, false);
          if (nextU != null) {
            let fields = "?$select=" + this.columnBuilder(r.prototype).columns;

            body += '--' + change + "\n";
            body += "Content-Type: application/http\n";
            body += "Content-Transfer-Encoding:binary\n";
            body += "Content-ID: " + count.toString() + "\n";
            body += "\n";
            body += "PATCH " + this.getContext().$devClientUrl() + r.prototype._pluralName + "(" + r.instance.id + ")" + fields + " HTTP/1.1\n";
            body += "Content-Type: application/json;type=entry\n";
            body += "\n";
            body += JSON.stringify(nextU) + "\n";
          }
        }
        count++;
      });
      body += '--' + change + '--\n';
      body += "\n";
      body += "--" + batch + "--\n";
      this.xrmService.log(body);

      let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + "$batch";
      this.xrmService.log(url);


      return this.http.post(this.forceHTTPS(url), body, { headers: headers, responseType: "text" }).pipe(map(_txt => {
        let txt = _txt as string;
        this.xrmService.log(txt);

        oprs.forEach(r => {
          if (r.type == 'put' && r.field != null) {
            this.assignValue(r.prototype, r.instance, r.field, r.value);
            this.updateCM(r.prototype, r.instance);
          }

          if (r.type == 'update') {
            this.updateCM(r.prototype, r.instance);
          }

          if (r.type == 'delete') {
            let key = r.instance._pluralName + ':' + r.instance.id;
            this.context.delete(key);
            delete this.changemanager[key];
          }
        });

        let index: number = 0;
        txt.split('\n').forEach(l => {
          if (l.startsWith('Content-ID:')) {
            index = Number(l.split(':')[1].trim());
            return true;
          }
          if (l.startsWith('OData-EntityId:')) {
            let opr = oprs.find(o => o.id == index);

            if (opr != null && opr.type == 'create') {
              let id = l.split('OData-EntityId:')[1].split('/' + opr.prototype._pluralName + '(')[1].replace(')', '').trim();
              opr.instance.id = id;
              let key = opr.prototype._pluralName + ':' + opr.instance.id;
              this.context.set(key, opr.instance);
              opr.instance._updateable = true;
              this.updateCM(opr.prototype, opr.instance);
            }
          }

          return true;
        });
        transaction.clear();
        return null;
      }));
    }
    throw 'you must parse at least one operation to the transaction by calling put, create, update or delete';
  }


  log(type: string): void {
    if (type == 'context') {
      this.xrmService.log(this.context);
      return;
    }

    if (type == 'xrmcontext') {
      this.xrmService.log(this.getContext());
      return;
    }

    if (type == 'url') {
      this.xrmService.log(this.getContext().getClientUrl());
    }

    if (type == 'version') {
      this.xrmService.log(this.getContext().getVersion());
    }

    this.xrmService.log('xrmContextService supported the current log types: context, xrmcontext, url, version');
  }

  clone(prototype: Entity, instance: Entity): Entity {
    let r = new Entity(prototype._pluralName, prototype._keyName);

    let _r = r as IndexedObject;

    for (let prop in prototype) {
      if (prototype.ignoreColumn(prop)) continue;

      let _prototype = prototype as IndexedObject;
      let _instance = instance as IndexedObject;

      let pv = _prototype[prop];
      if (typeof pv == 'function') {
        _r[prop] = pv;
        continue;
      }

      let v = _instance[prop];

      if (v == null) {
        _r[prop] = null;
        continue;
      }

      if (v instanceof Date) {
        _r[prop] = new Date(v.valueOf());
        continue;
      }

      if (v instanceof EntityReference) {
        _r[prop] = v.clone();
        continue;
      }

      if (v instanceof OptionSetValue) {
        _r[prop] = v.clone();
        continue;
      }

      if (v != null) {
        _r[prop] = v;
      }
    }
    r.id = '';
    this.xrmService.log('clone');
    this.xrmService.log(r);

    return r;
  }

  applyAccess(prototype: Entity, instance: Entity): Observable<Entity> | null {
    if (!prototype.hasOwnProperty('access')) throw 'The metadata must define a property "access" of type XrmAccess';

    let _instance = instance as IndexedObject;

    if (instance.hasOwnProperty('access') && _instance['access']['resolved']) {
      return new Observable(obs => obs.next(instance));
    }

    return this.mapAccess(prototype, instance);
  }

  private prepareUpdate(prototype: Entity, instance: Entity, deletedReferenceAsEmptyGuid: boolean): any {
    let me = this;
    let upd = new IndexedObject();

    let countFields = 0;

    let key = instance._pluralName + ':' + instance.id;
    let cm = this.changemanager[key];
    if (typeof cm === 'undefined' || cm === null) {
      throw 'the object is not under change control and cannot be updated within this context';
    }

    let _prototype = prototype as IndexedObject;
    let _instance = instance as IndexedObject;

    for (let prop in prototype) {
      if (prototype.hasOwnProperty(prop) && typeof _prototype[prop] != 'function' && _prototype[prop] !== undefined) {
        if (prototype.ignoreColumn(prop)) continue;
        let prevValue = cm[prop];

        
        let newValue = _instance[prop];

        if ((prevValue === 'undefined' || prevValue === null) && (newValue === 'undefined' || newValue === null)) continue;

        if (_instance[prop] instanceof EntityReference) {
          if (_instance[prop].associatednavigationproperty != null && _instance[prop].associatednavigationproperty != '' && _instance[prop]['pluralName'] != null && _instance[prop]['pluralName'] != '') {
            if (!EntityReference.same(prevValue, newValue)) {
              if (deletedReferenceAsEmptyGuid && newValue != null && (newValue["id"] == null || newValue["id"] == '')) {
                newValue["id"] = XRMCONTEXTSERVICE_EMPTY_GUID;
              }

              if (newValue != null && newValue["id"] != null && newValue["id"] != '') {
                let x = newValue["id"] as string;
                x = x.replace('{', '').replace('}', '');
                upd[_instance[prop]['associatednavigationpropertyname']()] = '/' + _instance[prop]['pluralName'] + '(' + x + ')';
              } else {
                // this does not work, navigation properties can only be removed with SDK deleted method
                upd["_" + _instance[prop]['logicalname'].toLowerCase() + "_value"] = null;
              }
              countFields++;
            }
            continue;
          }
        }

        if (_prototype[prop] instanceof EntityReference) {
          if (!EntityReference.same(prevValue, newValue)) {
            if ((newValue == null || newValue["id"] == null || newValue["id"] == "") && deletedReferenceAsEmptyGuid) {
              newValue["id"] = XRMCONTEXTSERVICE_EMPTY_GUID;
            }

            if (newValue != null && newValue["id"] != null && newValue["id"] != '') {
              let x = newValue["id"] as string;
              x = x.replace('{', '').replace('}', '');
              upd[_prototype[prop]['associatednavigationpropertyname']()] = '/' + _prototype[prop]['pluralName'] + '(' + x + ')';
            } else {
              upd["_" + _instance[prop]['logicalname'].toLowerCase() + "_value"] = null;
            }
            countFields++;
          }
          continue;
        }

        if (_prototype[prop] instanceof OptionSetValue) {
          if (!OptionSetValue.same(prevValue, newValue)) {
            let o = newValue as OptionSetValue;
            if (o == null || o.value == null) {
              upd[prop.toString()] = null;
            } else {
              upd[prop.toString()] = o.value;
            }
            countFields++;
          }
          continue;
        }

        if (_prototype[prop] instanceof Date) {
          if (prevValue instanceof Date && newValue instanceof Date) {
            var d1 = (prevValue as Date).toISOString();
            var d2 = (newValue as Date).toISOString();

            if (d1 == d2) {
              continue;
            }
          }

          if (prevValue != newValue) {

            this.xrmService.log('pre-value-update-date:[' + prevValue + ']/[' + newValue + ']');
            if (newValue == null) {
              upd[prop.toString()] = null;
            } else {
              if (newValue instanceof Date) {
                let d = newValue as Date;
                var sValue = d.toISOString();
                if (sValue.indexOf("T00:00:00.000Z") > 0) {
                  sValue = sValue.replace("T00:00:00.000Z", "");
                }
                upd[prop.toString()] = sValue;
              } else {
                upd[prop.toString()] = newValue;
              }
            }
            countFields++;
          }
          continue;
        }

        if (typeof _prototype[prop] == "number") {
          if (typeof newValue == "string") {
            if (newValue == "") {
              newValue = null;
            } else {
              if (isNum(newValue)) {
                newValue = Number(newValue);
              } else {
                newValue = prevValue;
              }
            }
          }

          if (prevValue != newValue) {
            upd[prop.toString()] = newValue;
            countFields++;
          }
          continue;
        }

        if (prevValue === true && newValue === true) {
          continue;
        }

        if (prevValue === false && newValue === false) {
          continue;
        }

        if (prevValue != newValue) {
          this.xrmService.log('pre-value-update:' + prop);
          this.xrmService.log(prevValue);

          this.xrmService.log('new-value-update:' + prop);
          this.xrmService.log(newValue);

          upd[prop.toString()] = _instance[prop];
          countFields++;
        }
      }
    }

    if (countFields > 0) {
      return upd;
    }
    return null;
  }

  /*
   * return an object that has been adjusted to patterns used in a webapi POST method. This is convinient in
   * combination with actions that uses parameters of type Entity, so the entity ex.
   */

  getCreatePayload(prototype: Entity, instance: Entity): any {
    return this.prepareNewInstance(prototype, instance);
  }

  /* 
   * return an object that has been adjused to pattern used in webapi PATCH method, for entity update. Only fields
   * that has actually changed will be present in the return instance
   */
  getUpdatePayload(prototype: Entity, instance: Entity): any
  getUpdatePayload(prototype: Entity, instance: Entity, deletedReferenceAsEmptyGuid: boolean): any
  getUpdatePayload(prototype: Entity, instance: Entity, deletedReferenceAsEmptyGuid: boolean = false): any {
    var next = this.prepareUpdate(prototype, instance, deletedReferenceAsEmptyGuid);

    if (next != null) {
      next[prototype._keyName] = instance.id;
      next["@odata.type"] = "Microsoft.Dynamics.CRM." + prototype._logicalName;
    }

    return next;
  }

  hasChanges(prototype: Entity, instance: Entity): boolean
  hasChanges(prototype: Entity, instance: Entity, deletedReferenceAsEmptyGuid: boolean): boolean
  hasChanges(prototype: Entity, instance: Entity, deletedReferenceAsEmptyGuid: boolean = false): boolean {
    return this.prepareUpdate(prototype, instance, deletedReferenceAsEmptyGuid) != null;
  }

  getEntityCollectionPayload(prototype: Entity, instances: Entity[]): any[] {
    let result: any[] = [];

    instances.forEach(e => {
      let next = this.prepareNewInstance(prototype, e);

      if (e.id != null && e.id != '') {
        next[prototype._keyName] = e.id;
      }

      next["@odata.type"] = "Microsoft.Dynamics.CRM." + prototype._logicalName;
      result.push(next);
    });

    return result;
  }

  private resolveAccess(prototype: Entity, instance: Entity) {
    var user = this.getContext().getUserId();
    if (user == null || user == '') {
      setTimeout(() => {
        this.resolveAccess(prototype, instance);
      }, 200);
      return;
    }

    this.mapAccess(prototype, instance)?.subscribe(r => { });
  }

  private mapAccess(prototype: Entity, instance: Entity): Observable<Entity> | null {
    var _prototype = prototype as IndexedObject;
    var _instance = instance as IndexedObject;

    if (!prototype.hasOwnProperty('access') || !(_prototype['access'] instanceof XrmAccess)) {
      return null;
    }

    if (!instance.hasOwnProperty('access')) {
      _instance['access'] = new XrmAccess();
    } else {
      let r = _instance['access'] as XrmAccess;
      if (r.resolved != null) {
        return null;
      }
    }

    var user = this.getContext().getUserId().replace('{', '').replace('}', '');

    let r = _instance['access'] as XrmAccess;
    r.resolved = false;

    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    headers = headers.append("Cache-Control", "no-cache");

    let url = this.getContext().getClientUrl() + this.xrmService.apiUrl + "systemusers(" + user + ")/Microsoft.Dynamics.CRM.RetrievePrincipalAccess(Target=@tid)?@tid={\"@odata.id\":\"" + prototype._pluralName + "(" + instance.id + ")\"}";
    this.xrmService.log(url);

    let _ta = _instance['access'] as XrmAccess;
    _ta.resolved = false;

    return this.http.get(this.forceHTTPS(url), { headers: headers }).pipe(
      map(r => {
        this.xrmService.log(r);

        let _r = r as IndexedObject;

        let i = _instance['access'] as XrmAccess;
        let perm = _r["AccessRights"] as string;
        // ReadAccess, WriteAccess, AppendAccess, AppendToAccess, CreateAccess, DeleteAccess, ShareAccess, AssignAccess
        i.append = perm.indexOf('AppendAccess') >= 0;
        i.appendTo = perm.indexOf('AppendToAccess') >= 0
        i.assign = perm.indexOf('AssignAccess') >= 0;
        i.create = perm.indexOf('CreateAccess') >= 0;
        i.delete = perm.indexOf('DeleteAccess') >= 0;
        i.read = perm.indexOf('ReadAccess') >= 0;
        i.share = perm.indexOf('ShareAccess') >= 0;
        i.write = perm.indexOf('WriteAccess') >= 0;
        i.resolved = true;
        return instance;
      })
    );
  }

  private preparePutValue(prototype: Entity, field: string, value: any): any {

    let _prototype = prototype as IndexedObject;

    let t = _prototype[field];

    if (t instanceof OptionSetValue) {
      if (value == null || value['value'] == null) {
        return { field: field, value: null, propertyAs: 'value' };
      } else {
        return { field: field, value: value['value'], propertyAs: 'value' };
      }
    }

    if (typeof t == 'number' && typeof value == 'string') {
      if (value == '') {
        value = null;
      } else {
        if (isNum(value)) {
          value = Number(value);
        } 
      }
    }

    if (typeof t == 'number' && typeof value == 'number') {
      if (value == null) {
        return { field: field, value: null, propertyAs: null };
      } else {
        // this is a really really stupid hack, because dynamics do not accept Integer for decimal fields, so we force 
        // a decimal position into the value before it is send.
        let rv = value + t;
        return { field: field, value: rv, propertyAs: 'value' };
      }
    }

    if (t instanceof EntityReference) {
      field = t.associatednavigationpropertyname().split('@')[0] + "/$ref";

      if (value.id == null || value.id == '') {
        return { field: field, value: null, propertyAs: '@odata.id', isDefault: false };
      } else {
        return { field: field, value: this.getContext().$devClientUrl() + t.pluralName + "(" + value.id + ")", propertyAs: '@odata.id', isdecimal: false };
      }
    }

    if (value instanceof Date) {
      return { field: field, value: value.toISOString(), propertyAs: 'value' };
    }

    return { field: field, value: value, propertyAs: 'value' };
  }

  private prepareNewInstance(prototype: Entity, instance: Entity): any {
    let newr = new IndexedObject();

    let _prototype = prototype as IndexedObject;
    let _instance = instance as IndexedObject;

    for (let prop in prototype) {
      if (prototype.hasOwnProperty(prop) && typeof _prototype[prop] !== 'function') {
        if (prototype.ignoreColumn(prop)) continue;

        let value = _instance[prop];
        if (value !== 'undefined' && value !== null) {

          if (_instance[prop] instanceof EntityReference) {
            if (_instance[prop].associatednavigationproperty != null && _instance[prop].associatednavigationproperty != '' && _instance[prop]['pluralName'] != null && _instance[prop]['pluralName'] != '') {
              let ref = _instance[prop] as EntityReference;
              if (ref != null && ref.id != null && ref.id != '') {
                newr[_instance[prop]['associatednavigationpropertyname']()] = '/' + _instance[prop]['pluralName'] + '(' + ref.id.replace('{', '').replace('}', '') + ')';
              }
              continue;
            }
          }

          if (_prototype[prop] instanceof EntityReference) {
            let ref = _instance[prop] as EntityReference;
            if (ref != null && ref.id != null && ref.id != '') {
              newr[_prototype[prop]['associatednavigationpropertyname']()] = '/' + _prototype[prop]['pluralName'] + '(' + ref.id.replace('{', '').replace('}', '') + ')';
            }
            continue;
          }

          if (_prototype[prop] instanceof OptionSetValue) {
            let o = _instance[prop] as OptionSetValue;
            if (o != null && o.value != null) {
              newr[prop.toString()] = o.value;
            }
            continue;
          }

          if (_prototype[prop] instanceof Date) {
            let d = value as Date;
            if (d != null) {
              newr[prop.toString()] = d.toISOString();
            }
            continue;
          }

          if (typeof _prototype[prop] == "number") {
            if (typeof value == "string") {
              if (value == "") {
                continue;
              }

              if (!isNum(value)) {
                continue;
              }
            }
            newr[prop.toString()] = value;
            continue;
          }

          newr[prop.toString()] = _instance[prop];
        }
      }
    }
    return newr;
  }

  private assignValue(prototype: Entity, instance: Entity, prop: string, value: any) {
    let _prototype = prototype as IndexedObject;
    let _instance = instance as IndexedObject;

    if (value != null) {
      _instance[prop] = value;
      return;
    }

    let t = _prototype[prop];
    if (t instanceof EntityReference) {
      _instance[prop] = t.clone();
      return;
    }

    if (t instanceof OptionSetValue) {
      _instance[prop] = new OptionSetValue();
      return;
    }

    _instance[prop] = null;
  }

  private $expandToExpand(prop: ExpandProperty): Expand | null {
    if (prop != null && prop.entity != null) {
      let result = new Expand();
      result.name = prop.name;
      result.select = this.columnBuilder(prop.entity).columns;
      return result;
    }
    return null;
  }

  private resolveFetchResult<T extends Entity>(prototype: T, response: any, top: number, pages: string[], pageIndex: number, fetchXml: Fetchxml): XrmQueryResult<T> {
    let me = this;
    var result = this.resolveQueryResult(prototype, response, top, pages, pageIndex, fetchXml.entity().aliasWithAttributes());

    var pageCookie = response["@Microsoft.Dynamics.CRM.fetchxmlpagingcookie"] as string;
    if (pageCookie != null && pageCookie != '') {

      this.xrmService.log('page-cookie');
      this.xrmService.log(pageCookie);

      var pac = "";
      var pc = pageCookie.match(this.pageCookieMatch);
      if (pc != null && pc.length > 0 && pc[0].length >= 14) {
         pac = pc[0].substring(14)
      }

      var fragment = decodeURIComponent(pac);
      fragment = fragment.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\"/g, "&quot;");

      this.xrmService.log("page-cookie-fragment");
      this.xrmService.log(fragment);

      let fetchxml = fetchXml.toFetchXml(fragment, pageIndex + 2);
      result.nextLink = this.getContext().getClientUrl() + this.xrmService.apiUrl + prototype._pluralName + "?fetchXml=" + encodeURIComponent(fetchxml);

      result.next = (): Observable<XrmQueryResult<T>> => {
        let headers = new HttpHeaders({ 'Accept': 'application/json' });
        headers = headers.append("OData-MaxVersion", "4.0");
        headers = headers.append("OData-Version", "4.0");
        headers = headers.append("Content-Type", "application/json; charset=utf-8");
        headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
        headers = headers.append("Cache-Control", "no-cache");

        let options = {
          headers: headers
        }

        if (result.nextLink != null) {
          return this.http.get(this.forceHTTPS(result.nextLink), options).pipe(map(response => {
            if (result.nextLink != null) { 
              pages.push(result.nextLink);
            }
            let re = me.resolveFetchResult<T>(prototype, response, top, pages, (pageIndex + 1), fetchXml);
            return re;
          }));
        } else {
          throw new Error("nextLink is null");
        }
      }

      if (pageIndex >= 1) {
        result.prev = (): Observable<XrmQueryResult<T>> => {
          let headers = new HttpHeaders({ 'Accept': 'application/json' });
          headers = headers.append("OData-MaxVersion", "4.0");
          headers = headers.append("OData-Version", "4.0");
          headers = headers.append("Content-Type", "application/json; charset=utf-8");
          headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
          headers = headers.append("Cache-Control", "no-cache");

          let options = {
            headers: headers
          }

          let lastPage = result.pages[result.pageIndex - 1];
          return me.http.get(me.forceHTTPS(lastPage), options).pipe(map(r => {
            result.pages.splice(result.pages.length - 1, 1);
            let pr = me.resolveFetchResult<T>(prototype, r, top, result.pages, result.pageIndex - 1, fetchXml);
            return pr;
          }));
        }
      }
    }
    return result;
  }

  private resolveQueryResult<T extends Entity>(prototype: T, response: any, top: number, pages: string[], pageIndex: number, alias?: string[]): XrmQueryResult<T> {
    let me = this;
    let result: XrmQueryResult<T> = {
      context: response["@odata.context"],
      count: response["@odata.count"],
      pages: pages,
      pageIndex: pageIndex,
      top: top,
      value: []
    }

    let vals = response["value"] as T[];

    vals.forEach(r => {
      let z = me.resolve(prototype, r, prototype._updateable, alias);
      result.value.push(z);
    });

    let nextLink = response["@odata.nextLink"] as string;

    if (nextLink != null && nextLink != '') {
      let start = nextLink.indexOf('/api');
      nextLink = me.getContext().getClientUrl() + nextLink.substring(start);
      result = {
        context: result.context,
        count: result.count,
        value: result.value,
        pages: pages,
        pageIndex: pageIndex,
        top: top,
        nextLink: nextLink,
        next: (): Observable<XrmQueryResult<T>> => {
          let headers = new HttpHeaders({ 'Accept': 'application/json' });
          headers = headers.append("OData-MaxVersion", "4.0");
          headers = headers.append("OData-Version", "4.0");
          headers = headers.append("Content-Type", "application/json; charset=utf-8");
          if (top > 0) {
            headers = headers.append("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=" + top.toString());
          } else {
            headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
          }
          headers = headers.append("Cache-Control", "no-cache");

          let options = {
            headers: headers
          }
          return me.http.get(me.forceHTTPS(nextLink), options).pipe(map(r => {
            pages.push(nextLink);
            let pr = me.resolveQueryResult<T>(prototype, r, top, pages, pageIndex + 1, alias);
            return pr;
          }));
        }
      }
    }

    if (result.pageIndex >= 1) {
      result.prev = (): Observable<XrmQueryResult<T>> => {
        let headers = new HttpHeaders({ 'Accept': 'application/json' });
        headers = headers.append("OData-MaxVersion", "4.0");
        headers = headers.append("OData-Version", "4.0");
        headers = headers.append("Content-Type", "application/json; charset=utf-8");
        headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
        if (top > 0) {
          headers = headers.append("Prefer", "odata.maxpagesize=" + top.toString());
        } else {
        }
        headers = headers.append("Cache-Control", "no-cache");

        let options = {
          headers: headers
        }

        let lastPage = result.pages[result.pageIndex - 1];
        return me.http.get(me.forceHTTPS(lastPage), options).pipe(map(r => {
          result.pages.splice(result.pages.length - 1, 1);
          let pr = me.resolveQueryResult<T>(prototype, r, top, result.pages, result.pageIndex - 1, alias);
          return pr;
        }));
      }
    }
    return result;
  }

  private resolveNewInstance<T extends Entity>(prototype: T, instance: any, result: any): void {
    let key = prototype._pluralName + ':' + instance[prototype._keyName];
    instance["id"] = result[prototype._keyName];
    instance["_pluralName"] = prototype._pluralName;
    instance["_logicalName"] = prototype._logicalName;
    instance["_keyName"] = prototype._keyName;
    this.context.set(key, instance);
  }

  private resolve<T extends Entity>(prototype: T, instance: any, updateable: boolean, alias?: string[]): T {
    let me = this;

    let _prototype = prototype as IndexedObject;

    this.xrmService.log(instance);

    let key = prototype._pluralName + ':' + instance[prototype._keyName];
    let result = {} as any;

    if (this.context.has(key)) {
      result = this.context.get(key);
      console.log(key + ' taken from cache');
    } else {
      console.log(key + ' created new instance');
      result["id"] = instance[prototype._keyName];
      result["_pluralName"] = prototype._pluralName;
      result["_logicalName"] = prototype._logicalName;
      result["_keyName"] = prototype._keyName;
      delete result[prototype._keyName];
      this.context.set(key, result);
    }

    if (this.includeOriginalPayload$) {
      result["_original$"] = instance;
    } else {
      if (result.hasOwnProperty("_original$")) {
        delete result["_original$"];
      }
    }

    result['_updateable'] = updateable;

    for (let prop in prototype) {
      if (prototype.ignoreColumn(prop)) continue;

      if (prototype.hasOwnProperty(prop) && typeof prototype[prop] != 'function') {
        let done = false;
        if (prototype[prop] instanceof EntityReference) {
          let ref = new EntityReference();
          let id = instance["_" + prop + "_value"] as string;
          if (id != null && id != 'undefined') {
            ref.id = id.toLowerCase();
            delete result["_" + prop + "_value"];

            ref.logicalname = instance["_" + prop + "_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
            delete instance["_" + prop + "_value@Microsoft.Dynamics.CRM.lookuplogicalname"];

            ref.name = instance["_" + prop + "_value@OData.Community.Display.V1.FormattedValue"];
            delete instance["_" + prop + "_value@OData.Community.Display.V1.FormattedValue"];

            ref.associatednavigationproperty = instance["_" + prop + "_value@Microsoft.Dynamics.CRM.associatednavigationproperty"];
            delete instance["_" + prop + "_value@Microsoft.Dynamics.CRM.associatednavigationproperty"];
          }
          result[prop] = ref;
          done = true;
        }

        if (!done && prototype[prop] instanceof OptionSetValue) {
          let opt = new OptionSetValue();
          opt.value = instance[prop];
          opt.name = instance[prop + '@OData.Community.Display.V1.FormattedValue'];
          result[prop] = opt;
          done = true;
        }

        if (!done && prototype[prop] instanceof Date) {
          let v = instance[prop];
          if (v != null && v != '') {
            result[prop] = new Date(Date.parse(v));
          } else {
            result[prop] = null;
          }

          done = true;
        }

        if (!done) {
          result[prop] = instance[prop];
          done = true;
        }
      }
    }

    var names = Object.getOwnPropertyNames(Object.getPrototypeOf(prototype));
    names.forEach(r => {
      if (r != 'constructor' && typeof _prototype[r] === 'function') {
        result[r] = _prototype[r];
      }
    });

    let eps = this.getExpandProperties(prototype);

    if (eps != null && eps.length > 0) {

      eps.forEach(ep => {
        if (ep != null) {
          if (ep.isArray) {
            let _v = instance[ep.name];
            if (_v != null && Array.isArray(_v)) {
              let _tmp:any[] = [];
              _v.forEach(_r => {
                _tmp.push(me.resolve(ep.entity, _r, false, alias));
              });
              result[ep.name] = _tmp;
              var _xtemp = _tmp as IndexedObject;
              if (ep.value instanceof Entities) {
                _xtemp["add"] = ep.value["add"];
                _xtemp["remove"] = ep.value["remove"];
                _xtemp["xrmService"] = this.xrmService;
                _xtemp["parentType"] = prototype._pluralName;
                _xtemp["parentId"] = result["id"];
                _xtemp["childType"] = ep.value["childType"];
                _xtemp["refName"] = ep.value["refName"];
                _xtemp["leftToRight"] = ep.value["leftToRight"];
              }
            }
          } else {
            let _v = instance[ep.name];
            if (_v != null) {
              result[ep.name] = this.resolve(ep.entity, _v, false, alias);
              result[ep.name]['_keyName'] = ep.entity._keyName;
              result[ep.name]['_pluralName'] = ep.entity._pluralName;
              result[ep.name]['_logicalName'] = ep.entity._logicalName;
            }
          }
        }
      });
    }

    if (alias != null && alias.length > 0) {
      alias.forEach(a => {
        for (var p in result) {
          if (p.startsWith(a + ".")) {
            delete result[p];
          }
        }
        for (var p in instance) {
          if (p.startsWith(a + '.')) {
            result[p] = instance[p];
          }
        }
      })
    }

    if (result['onFetch'] !== 'undefined' && result["onFetch"] != null && typeof result["onFetch"] === 'function') {
      result['onFetch']();
    }

    if (prototype.hasOwnProperty('access') && !_prototype['access']['lazy']) {
      if (!result.hasOwnProperty('access') || result.access.resolved == null) {
        this.resolveAccess(prototype, result);
      }
    }

    if (updateable) {
      this.updateCM(prototype, result);
    }

    return result as T;
  }


  private updateCM(prototype: any, instance: any): void {
    let key = prototype._pluralName + ':' + instance['id'];
    let change = new IndexedObject();

    this.xrmService.log('Adding to cm ' + key);

    this.changemanager[key] = change;

    for (let prop in prototype) {
      if (prototype.ignoreColumn(prop)) continue;
      if (prototype.hasOwnProperty(prop) && typeof prototype[prop] != 'function') {
        let v = instance[prop];
        if (v == null) continue;

        let done = false;

        if (v instanceof EntityReference) {
          change[prop] = v.clone();
          done = true;
        }

        if (!done && v instanceof OptionSetValue) {
          change[prop] = v.clone();
          done = true;
        }

        if (!done && v instanceof Date) {

          change[prop] = new Date(v.valueOf());
          done = true;
        }

        if (!done) {
          change[prop] = v;
          done = true;
        }
      }
    }
    this.xrmService.log(change);
  }

  private columnBuilder(entity: Entity): ColumnBuilder {
    var columns = entity.columns(true);
    let result = new ColumnBuilder(columns.join(","));
    result.hasEntityReference = columns.filter(r => r.startsWith("_") && r.endsWith("_value")).length > 0;
    return result;
  }

  private getExpandProperties(entity: Entity): ExpandProperty[] {
    var result = [];
    var _entity = entity as IndexedObject;
    for (var prop in entity) {
      if (prop == entity._keyName) continue;
      if (entity.ignoreColumn(prop)) continue;

      let _v = _entity[prop];
      if (Array.isArray(_v)) {
        if (_v.length > 0) {
          let pt = _v[0] as Entity;
          result.push({
            name: prop,
            entity: pt,
            isArray: true,
            value: _v
          });
        }
      } else {
        if (_v instanceof Entity) {
          result.push({
            name: prop,
            entity: _v,
            isArray: false,
            value: _v
          });
        }
      }
    }
    return result;
  }

  private toFuncParameterString(pam: any): string {
    if (typeof pam === "string") return pam;

    let r = '(';
    var ix = 1;
    var cm = '';
    for (var p in pam) {
      if (pam.hasOwnProperty(p)) {
        r += cm + p + "=@p" + ix;
        ix++;
        cm = ',';
      }
    }
    r += ')';

    ix = 1;
    cm = '?';
    for (var p in pam) {
      if (pam.hasOwnProperty(p)) {
        r += cm + '@p' + ix + "=";

        ix++;
        cm = "&";


        var v = pam[p];

        if (v == null) {
          v = '';
          r += v;
          continue;
        }


        var valueWrapper = v["functionPropertyValueAsString"];
        if (valueWrapper != null) {
          r += valueWrapper.call(v);
          continue;
        }

        if (v.hasOwnProperty("toJsonProperty")) {
          v = v["toJsonProperty"]();
        }

        if (v instanceof Date) {
          r += v.toISOString();
          continue;
        }

        if (typeof v === "number") {
          r += v.toString();
          continue;
        }

        if (typeof v === "boolean") {
          r += v ? "true" : "false";
          continue;
        }

        if (typeof v === "string") {
          r += "'" + v + "'";
          continue;
        }

        r += JSON.stringify(this.transform(v));
      }
    }
    return r;
  }

  private transform(input: any): any {
    var transformed = false;
    var result = new IndexedObject();

    for (var pam in input) {
      if (input.hasOwnProperty(pam)) {
        var value = input[pam];
        if (value != null && value.hasOwnProperty("toJsonProperty")) {
          result[pam] = value["toJsonProperty"]();
          transformed = true;
        } else {
          result[pam] = value;
        }
      }
    }

    if (!transformed) {
      return input;
    }

    return result;
  }

  private forceHTTPS(v: string): string {
    return this.xrmService.forceHTTPS(v);
  }
}
