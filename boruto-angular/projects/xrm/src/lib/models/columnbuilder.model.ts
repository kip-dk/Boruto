export class ColumnBuilder {
    constructor(columns: string, hasEntityReference?: boolean) {
      this.columns = columns;
      if (hasEntityReference != null) {
        this.hasEntityReference = hasEntityReference;
      }
    }
  
    columns: string;
    hasEntityReference: boolean = false;
  }
  