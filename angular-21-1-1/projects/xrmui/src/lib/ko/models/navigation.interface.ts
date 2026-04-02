
export interface KoNavigation {
    id: string;
    title: string | null;
    disabled: boolean;
    click: any;
    over: boolean | null;
    cssClass: string | null;
    imgUrl: string | null;
  }
  
  export class KoDivider implements KoNavigation {
    id: string;
    title: string | null;
    disabled: boolean;
    click: any = null;
    over: boolean | null = null;
    cssClass: string | null = null;
    imgUrl: string | null = null;
  
    constructor() {
      this.id = "##divider##";
      this.title = "";
      this.disabled = true;
    }
  }
  
  export class KoNavigationByCssClass implements KoNavigation {
    id: string;
    title: string | null = null;
    disabled: boolean;
    cssClass: string;
    click: any;
    over: boolean | null = null;
    imgUrl: string | null = null;
  
    constructor(id: string, cssClass: string)
    constructor(id: string, cssClass: string, disabled: boolean)
    constructor(id: string, cssClass: string, disabled: boolean, title: string)
    constructor(id: string, cssClass: string, disabled: boolean, title: string, click: any)
    constructor(id: string, cssClass: string, disabled: boolean = false, title: string | null = null, click: any = null ) {
      this.id = id;
      this.title = title;
      this.disabled = disabled;
      this.cssClass = cssClass;
      this.click = click;
    }
  }
  
  export class KoNavigationByImageUrl implements KoNavigation {
    id: string;
    title: string | null = null;
    disabled: boolean;
    imgUrl: string;
    click: any;
    over: boolean | null = null;
    cssClass: string | null = null;
  
    constructor(id: string, imgUrl: string)
    constructor(id: string, imgUrl: string, disabled: boolean)
    constructor(id: string, imgUrl: string, disabled: boolean, title: string)
    constructor(id: string, imgUrl: string, disabled: boolean, title: string, click: any)
    constructor(id: string, imgUrl: string, disabled: boolean = false, title: string | null = null, click: any = null) {
      this.id = id;
      this.title = null;
      this.disabled = disabled;
      this.imgUrl = imgUrl;
      this.click = null;
    }
  }
  