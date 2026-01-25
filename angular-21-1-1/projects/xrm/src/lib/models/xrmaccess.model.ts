export class XrmAccess {
    private lazy: boolean = true;
    resolved: boolean = false;
    read?: boolean;
    write?: boolean;
    append?: boolean;
    appendTo?: boolean;
    create?: boolean;
    delete?: boolean;
    share?: boolean;
    assign?: boolean;
  
    constructor();
    constructor(lasy: boolean);
    constructor(lazy: boolean = false) {
      this.lazy = lazy;
    }
  }
  