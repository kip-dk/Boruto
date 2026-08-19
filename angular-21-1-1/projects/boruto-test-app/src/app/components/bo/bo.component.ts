import { DatePipe, NgClass } from '@angular/common';
import { Component, computed, inject, Signal, signal, ChangeDetectionStrategy } from '@angular/core';
import { BorutoXrmUIModule, IMenu, ISearchService, ISelectable } from 'boruto-xrmui';

@Component({
  selector: 'app-bo',
  templateUrl: './bo.component.html',
  styleUrl: './bo.component.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports:[BorutoXrmUIModule,DatePipe]
})
export class BoComponent {

  description = signal<string>('');

  search = signal<string>('');
  dato = signal<Date | null>(new Date());

  tabs: IMenu[] = [
      { id: 1, label: "tab 1", icon: "search" },
      { id: 1, label: "tab 2", icon: "home" }
  ];

  selected = signal<IMenu>(this.tabs[0]);

  yesno = signal<boolean>(true);

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

  mynumber = signal<number>(1200000.45);

  toolClick() {
    alert('hi from tool click');
  }
}


export class SearchService implements ISearchService {

    private data: ISelectable[] = [
      {id:'0', name: "Ananas", completeOnly: true},
      { id: '1', name: 'Ananas', expandable: true, expand: async () => [
        { id: '1.1', name: 'Ananas 1' },
        { id: '1.2', name: 'Ananas 2' },
        { id: '1.3', name: 'Ananas 3' }
      ] },
      { id: '2', name: 'Apelsin' },
      { id: '3', name: 'Banan' },
      { id: '4', name: 'Citron' },
      { id: '5', name: 'Kiwi' },
      { id: '6', name: 'Pære' },
      { id: '7', name: 'Æble' }
    ];

    items = signal<ISelectable[]>(this.data);


  search(v: string): Promise<void> {
    if (!v || v.length == 0)  {
      this.items.set(this.data);
      Promise.resolve();
    }
    this.items.set(this.data.filter(r => r.name && r.name.toLocaleLowerCase().indexOf(v.toLocaleLowerCase()) >= 0));
    return Promise.resolve();
  }
}
