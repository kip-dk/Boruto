export class Expand {
    constructor(name?: string, select?: string, filter?: string, additional?: Expand[]) {
      this.name = name;
      this.select = select;
      this.filter = filter;
      this.additional = additional;
    }
    name?: string;
    select?: string;
    filter?: string;
    additional?: Expand[];
  
    toExpandString(): string {
      let _ex = this.name;
      if (this.select != null || this.filter != null) {
        _ex += '(';
      }
      let semi = '';
      if (this.select != null) {
        _ex += '$select=' + this.select;
        semi = ';'
      }
  
      if (this.filter != null) {
        _ex += semi + '$filter=' + this.filter;
      }
  
      if (this.select != null || this.filter != null) {
        _ex += ')';
      }
      return _ex ?? "";
    }
  }
  