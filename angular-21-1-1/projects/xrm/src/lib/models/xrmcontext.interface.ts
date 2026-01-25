export interface XrmContext {
    getClientUrl(): string;
    getQueryStringParameters(): any;
    getVersion(): string;
    getUserName(): string;
    getUserId(): string;
    $devClientUrl(): string;
  }
  