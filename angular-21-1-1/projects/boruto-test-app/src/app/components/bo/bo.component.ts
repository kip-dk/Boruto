import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { BorutoXrmUIModule, IMenu, ISearchService, ISelectable } from 'boruto-xrmui';

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

  tabs: IMenu[] = [
      { id: 1, label: "tab 1", icon: "search" },
      { id: 1, label: "tab 2", icon: "home" }
  ];

  selected = signal<IMenu>(this.tabs[0]);


  searchService = new SearchService();

  select(c: ISelectable | undefined) {
    this.search.set(c?.name ?? '');
  }

    choices: ISelectable[] = [
      { id: '1', name: 'København' },
      { id: '2', name: 'Århus' },
      { id: '3', name: 'Aalborg' }
  ];


  choice = signal<ISelectable>(this.choices[1]);

  choiceName = computed(() => this.choice().name);

  text = signal<string>('');
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
