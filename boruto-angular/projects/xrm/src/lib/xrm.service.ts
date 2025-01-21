import { Injectable, Injector } from "@angular/core";
import { XrmContext } from "./models/xrmcontext.interface";
import { HttpClient, HttpHeaders, HttpResponse } from "@angular/common/http";
import { IndexedObject } from "./models/indexedobject.model";
import { BehaviorSubject, lastValueFrom, map, Observable, Subscriber } from "rxjs";
import { XrmEntityKey } from "./models/xrmentitykey.model";
import { XrmFormKey } from "./models/xrmformkey.model";
import { Expand } from "./models/expand.model";
import { XrmQueryResult } from "./models/xrmqueryresult.interface";
import { XrmFormService } from "./xrmform.service";


@Injectable()
export class XrmService {
  private defaultApiUrl: string = "/api/data/v9.2/";
  private contextFallback?: XrmContext;
  apiUrl: string = '/api/data/v9.2/';
  apiVersion: string = 'v9.2';
  debug: boolean = false;
  forceHttps: boolean = false;
  private keyTries: number = 0;
  private currentUserId: string = '';
  private currentUserId$: BehaviorSubject<string>;

  constructor(private http: HttpClient, private injector: Injector, private xrmformService: XrmFormService) {
    this.currentUserId$ = new BehaviorSubject<string>('');
  }

  setVersion(v: string): void {
    this.apiUrl = this.defaultApiUrl.replace("9.2", v);
  }


  getContext(): XrmContext {
    if (this.contextFallback != null) {
      return this.contextFallback;
    }

    var x: XrmContext | null = this.xrmformService.getContext();

    if (x != null) {
      if (typeof x.getVersion == 'undefined') {
        x.getVersion = (): string => "9.2.0.0"
      }

      if (typeof x.$devClientUrl == 'undefined') {
        x.$devClientUrl = () => { return this.getContext().getClientUrl() + this.apiUrl };
        this.contextFallback = x;
        this.initializeVersion(this.contextFallback.getVersion());
      }
      return x;
    }

    this.log('using fake context');
    let baseUrl = "http://localhost:4200";
    let version = 'v9.2';

    var comesfrom = window.location.href.toLowerCase();
    if (comesfrom.indexOf("webresources") >= 0) {
      var domain = window.location.href.split('/')[2];
      var proto = comesfrom.startsWith("https://") ? "https://" : "http://";

      // to-do, find a better way to render multi org sub url from the url. (for now - never needed on online, therefore blank for https, but might be needed for onpre running https ... )
      var sub = comesfrom.startsWith("https://") ? "" : "/" + window.location.href.split('/')[3];
      baseUrl = proto + domain + sub;
    }

    this.contextFallback = {
      getClientUrl(): string {
        return baseUrl;
      },
      getQueryStringParameters(): any {
        let search = window.location.search;
        let hashes = search.slice(search.indexOf('?') + 1).split('&');
        let params: { [key: string]: any } = {};
        hashes.map(hash => {
          let [key, val] = hash.split('=')
          params[key] = decodeURIComponent(val)
        })
        return params;
      },
      getVersion(): string {
        return version.substring(1) + '.0.0';
      },
      getUserId(): string {
        var ix = this as IndexedObject;
        return ix["userid"];
      },
      getUserName(): string {
        var ix = this as IndexedObject;
        return ix["username"];
      },
      $devClientUrl(): string {
        var ix = this as IndexedObject;
        return ix['$clienturl'];
      }
    };

    var headers = this.getDefaultHeader();

    this.http.get <IndexedObject>(this.forceHTTPS(baseUrl) + "/api/data/" + version + "/WhoAmI()", { headers: headers }).subscribe(r => {
      this.log(r);
      let url = r['@odata.context'] as string;

      let firstpart = url.split('/api/data/')[0];
      let version = url.split('/api/data/')[1].split('/')[0];

      let cf = this.contextFallback as IndexedObject;

      cf["$clienturl"] = firstpart + '/api/data/' + version + '/';

      cf["userid"] = r["UserId"];
      cf["username"] = "Dev. fallback from whoami";
      if (this.contextFallback != null) {
        this.initializeVersion(this.contextFallback.getVersion());
      }
    });
    return this.contextFallback;
  }

  getCurrentUserId(): BehaviorSubject<string> {
    this.initializeCurrentUser();
    return this.currentUserId$;
    
  }

  private async initializeCurrentUser() {
    var ctx = this.getContext();
    if (ctx.getUserId() == null || ctx.getUserId() == '') {
      var headers = this.getDefaultHeader();
      var url = this.forceHTTPS(ctx.getClientUrl()) + this.apiUrl + "/WhoAmI()";
      this.log(url);

      var user = await lastValueFrom(this.http.get<IndexedObject>(url, { headers: headers }).pipe(map(r => { return r["UserId"] as string })));
      this.currentUserId$.next(user);
    } else {
      this.log('get user from normal crm context');
      this.log('should result in ' + ctx.getUserId());
      this.currentUserId$.next(ctx.getUserId());
    }
  }

  getCurrentKey(): Observable<XrmEntityKey>;
  getCurrentKey(repeatForCreateForm: boolean): Observable<XrmEntityKey>;
  getCurrentKey(repeatForCreateForm: boolean = false): Observable<XrmEntityKey> {
    // First we try to find id and type in the url, this allow forcing a specific entity, directly from standard url parameters
    let params = this.getQueryStringParameters();
    let result = new XrmEntityKey(params["id"], params["typename"]);


    if (result.id == null || result.id == '') {
      params = this.getContext().getQueryStringParameters();
      result.id = params["id"];
      result.entityType = params["typename"];
    }

    if (this.xrmformService.getFormType() == 1 && repeatForCreateForm) {
      var obs = new Observable<XrmEntityKey>(sub => {
        this.tryGetKey(sub);
      });
      return obs;
    }


    let key: XrmFormKey = this.xrmformService.getFormKey(result.id, result.entityType);
    if (key != null && key.id != null && key.type != null) {
      result.id = this.toGuid(key.id);
      result.entityType = key.type;
      return new Observable(obs => obs.next(result));
    }

    if (this.xrmformService.getFormType() == 2) {
      key = this.xrmformService.getFormKey(result.id, result.entityType);
      if (key != null && key.id != null && key.type != null) {
        result.id = this.toGuid(key.id);
        result.entityType = key.type;
        return new Observable(obs => obs.next(result));
      } else {
        return new Observable(obs => {
          var iv = setInterval(() => {
            let key = this.xrmformService.getFormKey(result.id, result.entityType);
            if (key != null && key.id != null && key.type != null) {
              clearInterval(iv);
              result.id = this.toGuid(key.id);
              result.entityType = key.type;
              obs.next(result);
            }
          }, 800);
        });
      }
    } else {
      // it was not an entity form, or the entity form was not in edit mode.
      result.id = this.toGuid(result.id);
      return new Observable<XrmEntityKey>(obs => obs.next(result));
    }
  }

  get<T>(entityTypes: string, id: string, fields: string, expand?: Expand): Observable<T> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");
    headers = headers.append("Cache-Control", "no-cache");

    let addFields = '';
    let sep = '?';

    if (fields != null && fields != '') {
      addFields = sep + "$select=" + fields;
      sep = '&';
    }

    let _ex = this.expandString(expand, sep);

    let _id = id.replace("{", "").replace("}", "");

    let url = this.getContext().getClientUrl() + this.apiUrl + entityTypes + "(" + _id + ")" + addFields + _ex;
    this.log(url);

    return this.http.get<T>(this.forceHTTPS(url), { headers: headers });
  }

  query<T>(entityTypes: string, fields: string, filter: string): Observable<XrmQueryResult<T>>;
  query<T>(entityTypes: string, fields: string, filter: string, orderBy: string): Observable<XrmQueryResult<T>>;
  query<T>(entityTypes: string, fields: string, filter: string, orderBy: string, top: number): Observable<XrmQueryResult<T>>;
  query<T>(entityTypes: string, fields: string, filter: string, orderBy?: string, top?: number, count?: boolean): Observable<XrmQueryResult<T>> {
    let me = this;
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    if (top != null && top > 0) {
      headers = headers.append("Prefer", "odata.maxpagesize=" + top.toString());
    } else {
    }
    headers = headers.append("Cache-Control", "no-cache");

    let url = this.getContext().getClientUrl() + this.apiUrl + entityTypes;
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

    return this.http.get(this.forceHTTPS(url), { headers: headers }).pipe(map(response => {
      let result = me.resolveQueryResult<T>(response, top ?? 250, [url], 0);
      return result;
    }));
  }

  create<T>(entityType: string, entity: T): Observable<T> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");
    headers = headers.append("Prefer", "return=representation");

    let options = {
      headers: headers,
    }

    return this.http.post(this.forceHTTPS(this.getContext().getClientUrl()) + this.apiUrl + entityType, entity, { headers: headers, observe: "response" }).pipe(map(
      (res: HttpResponse<any>) => {
        if (res.body == null) {
          let entityId = res.headers.get('OData-EntityId') as string;
          let result = { id: entityId.split('(')[1].replace(')', ''), $keyonly: true };
          return result;
        } else {
          return res.body;
        }
      }
    ));
  }

  update<T>(entityType: string, entity: T, id: string, fields?: string): Observable<T> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");
    headers = headers.append("Prefer", "return=representation");

    let _f = '';
    if (fields != null) {
      _f = '?$select=' + fields;
    }

    let fUrl = this.getContext().getClientUrl() + this.apiUrl + entityType + "(" + id + ")" + _f;
    this.log(fUrl);

    return this.http.patch(this.forceHTTPS(fUrl), entity, { headers: headers }).pipe(map(response => {
      if (response == null || this.getContext().getVersion().startsWith("8.0") || this.getContext().getVersion().startsWith("8.1")) {
        return entity;
      }
      return response as T;
    }));
  }

  put(entityType: string, id: string, field: string, value: any): Observable<null>;
  put(entityType: string, id: string, field: string, value: any, propertyValueAs?: string): Observable<null> {
    if (propertyValueAs == null) propertyValueAs = 'value';

    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");

    let v: IndexedObject = {
    };

    v[propertyValueAs] = value;
    this.log(v);


    let url = this.getContext().getClientUrl() + this.apiUrl + entityType + "(" + id + ")/" + field;
    this.log(url);

    if (v[propertyValueAs] != null) {
      return this.http.put(this.forceHTTPS(url), v, { headers: headers }).pipe(map(response => null));
    } else {
      return this.http.delete(this.forceHTTPS(url), { headers: headers }).pipe(map(response => null));
    }
  }

  delete(entityType: string, id: string): Observable<null> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");

    let url = this.getContext().getClientUrl() + this.apiUrl + entityType + "(" + id + ")";
    this.log(url);
    return this.http.delete(this.forceHTTPS(url), { headers: headers }).pipe(map(response => null));
  }

  getParameter(param: string): string {
    return this.getQueryStringParameters()[param];
  }

  associate(fromType: string, fromId: string, toType: string, toId: string, refname: string): Observable<null> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=*");

    let url = this.getContext().getClientUrl() + this.apiUrl + fromType + "(" + this.toGuid(fromId) + ")/" + refname + "/$ref";

    let data = {
      "@odata.id": this.getContext().$devClientUrl() + toType + "(" + this.toGuid(toId) + ")"
    }

    if (this.debug) {
      this.log(url);
      this.log(data);
    }

    return this.http.post(this.forceHTTPS(url), data, { headers: headers }).pipe(map(response => null));
  }

  disassociate(fromType: string, fromId: string, toType: string, toId: string, refname: string): Observable<null> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");

    let url = this.getContext().getClientUrl() + this.apiUrl + fromType + "(" + this.toGuid(fromId) + ")/" + refname + "/$ref?$id=" + this.getContext().$devClientUrl() + toType + "(" + this.toGuid(toId) + ")";
    this.log(url);

    return this.http.delete(this.forceHTTPS(url), { headers: headers }).pipe(map(response => {
      this.log(response);
      return null;
    }));
  }

  func(name: string, data: string, boundType?: string, boundId?: string): Observable<any> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");

    let url = this.getContext().getClientUrl() + this.apiUrl + name;
    if (boundType != null && boundId != null) {
      url = this.getContext().getClientUrl() + this.apiUrl + boundType + "(" + this.toGuid(boundId) + ")/" + name;
    }

    if (data != null && data.length > 0) {
      url += data;
    } else {
      url += "()";
    }

    this.log(url);
    return this.http.get(this.forceHTTPS(url), { headers: headers }).pipe(map(response => {
      this.log(response);
      return response;
    }));
  }


  action(name: string, data: any, boundType?: string, boundId?: string): Observable<any> {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("Content-Type","application/json; charset=utf-8");
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");

    let prefix = '';
    if (boundId != undefined && boundId != undefined) {
      prefix = 'Microsoft.Dynamics.CRM.';
    }

    let url = this.getContext().getClientUrl() + this.apiUrl + name;
    if (boundType != null && boundId != null) {
      url = this.getContext().getClientUrl() + this.apiUrl + boundType + "(" + this.toGuid(boundId) + ")/" + prefix + name;
    }
    this.log(url);

    var finalUrl = this.forceHTTPS(url);
    return this.http.post(finalUrl, data, { headers: headers }).pipe(map(response => {
      this.log(response);
      return response;
    }));
  }

  log(message: any): void {
    if (this.debug) {
      if (typeof message == 'string') {
        console.dir(message);
      } else {
        console.log(message);
      }
    }
  }


  private tryGetKey(sub: Subscriber<XrmEntityKey>): void {
    setTimeout(() => {
      var key = this.xrmformService.getFormKey('', '', true);
      if (key != null && key.id != null && key.id != '' && key.type != null && key.type != '') {
        let result = new XrmEntityKey(key.id, key.type);
        sub.next(result);
      } else {
        this.keyTries++;
        if (this.keyTries < 900) {
          // continue try for 1/2 hour at most
          this.tryGetKey(sub);
        }
      }
    }, 2000);
  }

  private initializeVersion(_v: string): void {
    if (_v == null) { _v = '9.0.0.0'; }
    let v = _v.split('.');
    this.setVersion(v[0] + "." + v[1]);
    this.apiVersion = 'v' + v[0] + '.' + v[1];
  }

  private expandString(expand?: Expand, sep?: string): string {
    if (expand == null || expand.name == null || expand.name == '') return '';

    let _ex = sep + '$expand=' + expand.toExpandString();

    if (expand.additional != null && expand.additional.length > 0) {
      expand.additional.forEach(ad => {
        _ex += "," + ad.toExpandString();
      });
    }
    return _ex;
  }

  private resolveQueryResult<T>(response: any, top: number, pages: string[], pageIndex: number): XrmQueryResult<T> {
    let me = this;
    let result: XrmQueryResult<T> = {
      context: response["@odata.context"],
      count: response["@odata.count"],
      value: response["value"] as T[],
      pages: pages,
      pageIndex: pageIndex,
      top: top
    }

    let nextLink = response["@odata.nextLink"] as string;

    if (nextLink != null && nextLink != '') {
      let start = nextLink.indexOf('/api');
      nextLink = me.getContext().getClientUrl() + nextLink.substring(start);
      result = {
        context: result.context,
        count: response["@odata.count"],
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
          headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
          if (top > 0) {
            headers = headers.append("Prefer", "odata.maxpagesize=" + top.toString());
          }
          headers = headers.append("Cache-Control", "no-cache");

          return me.http.get(me.forceHTTPS(nextLink), { headers: headers }).pipe(map(r => {
            pages.push(nextLink);
            let pr = me.resolveQueryResult<T>(r, top, pages, pageIndex + 1);
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

        let lastPage = result.pages[result.pageIndex - 1];
        return me.http.get(me.forceHTTPS(lastPage), { headers: headers }).pipe(map(r => {
          result.pages.splice(result.pages.length - 1, 1);
          let pr = me.resolveQueryResult<T>(r, top, result.pages, result.pageIndex - 1);
          return pr;
        }));
      }
    }
    return result;
  }

  private getQueryStringParameters(): any {
    let search = window.location.search;
    let hashes = search.slice(search.indexOf('?') + 1).split('&');
    let params: IndexedObject = {};
    hashes.map(hash => {
      let [key, val] = hash.split('=')
      params[key] = decodeURIComponent(val)
    })
    return params;
  }

  private toGuid(v: string): string {
    // 5C48BB2A-BFC0-4E56-A262-8494E0F6A8FD
    if (v == null || v == '') {
      return v;
    }

    v = decodeURIComponent(v).replace('{', '').replace('}', '');

    if (v.indexOf('-') >= 0) {
      return v;
    }
    return v.substring(0, 8) + '-' + v.substring(9, 13) + '-' + v.substring(14, 18) + '-' + v.substring(19, 23) + '-' + v.substring(24,36);
  }

  private getDefaultHeader(): HttpHeaders {
    let headers = new HttpHeaders({ 'Accept': 'application/json' });
    headers = headers.append("OData-MaxVersion", "4.0");
    headers = headers.append("OData-Version", "4.0");
    headers = headers.append("Content-Type", "application/json; charset=utf-8");
    headers = headers.append("Prefer", "odata.include-annotations=\"*\"");
    return headers;
  }


  forceHTTPS(v: string): string {
    if (this.forceHttps && v.startsWith("http://")) {
      return v.replace("http://", "https://");
    }
    return v;
  }
}
