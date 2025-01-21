import { Observable } from "rxjs";

export interface XrmQueryResult<T> {
    pages: string[];
    pageIndex: number;
    top: number;
    nextLink?: string;
    context: string;
    count: number;
    value: T[];
    prev?(): Observable<XrmQueryResult<T>>;
    next?(): Observable<XrmQueryResult<T>>;
  }
  