import { NativeDateAdapter } from "@angular/material/core";

export class DKCustomDateAdapter extends NativeDateAdapter {

  override format(date: Date | null, displayFormat: Object): string {
    if (!date) return '';

    const day = this.pad(date.getDate());
    const month = this.pad(date.getMonth() + 1);
    const year = date.getFullYear();

    return `${day}-${month}-${year}`;
  }

  override parse(value: any): Date | null {
    if (typeof value !== 'string' || !value.trim()) return null;

    const parts = value.split('-');
    if (parts.length !== 3) return null;

    const day = Number(parts[0]);
    const month = Number(parts[1]) - 1;
    const year = Number(parts[2]);

    const date = new Date(year, month, day);

    return isNaN(date.getTime()) ? null : date;
  }

  private pad(n: number): string {
    return n < 10 ? `0${n}` : `${n}`;
  }
}
