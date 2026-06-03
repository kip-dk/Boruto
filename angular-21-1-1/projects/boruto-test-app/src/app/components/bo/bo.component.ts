import { DatePipe, NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { BorutoXrmUIModule, ISearchService, ISelectable } from 'boruto-xrmui';

@Component({
  selector: 'app-bo',
  templateUrl: './bo.component.html',
  styleUrl: './bo.component.scss',
  imports:[BorutoXrmUIModule,DatePipe]
})
export class BoComponent {

  description = signal<string>('');

  search = signal<string>('');
  dato = signal<Date | null>(null);


  searchService = new SearchService();

  select(c: ISelectable | undefined) {
    this.search.set(c?.name ?? '');
  }
}


export class SearchService implements ISearchService {
    data: ISelectable[] = [
      { id: '1', name: 'Apple' },
      { id: '2', name: 'Banana' },
      { id: '3', name: 'Orange' }
    ];

  search(v: string): Promise<ISelectable[]> {
    return Promise.resolve(
      this.data.filter(x =>
        x.name != undefined && x.name.toLowerCase().includes(v.toLowerCase())
      )
    );
  }
}
