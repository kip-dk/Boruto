import { XrmContext } from "./xrmcontext.interface";

export class XrmContextInstance implements XrmContext {
    private clientUrl: string;
    private queryStringParameters: any;
    private version: string;
    private userName: string;
    private userId: string;
    private apiurl: string = '/api/data/v9.2/';
  
    constructor(ctx: XrmContext) {
      this.clientUrl = ctx.getClientUrl();
      this.queryStringParameters = ctx.getQueryStringParameters();
      this.version = ctx.getVersion();
      this.userName = ctx.getUserName();
      this.userId = ctx.getUserId();
    }
  
    getClientUrl(): string {
      return this.clientUrl;
    }
  
    getQueryStringParameters(): any {
      return this.queryStringParameters;
    }
  
    getVersion(): string {
      return this.version;
    }
  
    getUserName(): string {
      return this.userName;
    }
  
    getUserId(): string {
      return this.userId;
    }
    $devClientUrl(): string {
      return this.clientUrl + this.apiurl;;
    }
  }
  