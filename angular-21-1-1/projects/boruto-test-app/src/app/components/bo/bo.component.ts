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
      { id: '1', name: 'Ananas' },
      { id: '2', name: 'Apelsin' },
      { id: '3', name: 'Banan' },
      { id: '4', name: 'Citron' },
      { id: '5', name: 'Kiwi' },
      { id: '6', name: 'Pære' },
      { id: '7', name: 'Æble' }
    ];

  search(v: string): Promise<ISelectable[]> {
    return Promise.resolve(
      this.data.filter(x =>
        x.name != undefined && x.name.toLowerCase().includes(v.toLowerCase())
      )
    );
  }
}
