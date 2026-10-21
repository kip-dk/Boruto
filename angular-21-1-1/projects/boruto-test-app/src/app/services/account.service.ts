import { inject, Injectable, signal } from "@angular/core";
import { Account } from "../entities/account.entity";
import { ISearchService, ISelectable } from "boruto-xrmui";
import { Comparator, Condition, XrmContextService } from "boruto-xrmservice";
import { lastValueFrom, map } from "rxjs";


@Injectable({ providedIn: "root" })
export class AccountService {
  private readonly localPrototype = new Account().meta();
  private readonly xrmService = inject(XrmContextService);

  private accountSearch$ = signal<ISelectable[]>([]);
  readonly accountSearch: ISearchService;

  constructor() {
    this.accountSearch = this.getAccountSearch();
  }

  active(): Promise<Account[]> {
    return lastValueFrom(this.xrmService.query(this.localPrototype, new Condition().isActive(), "name").pipe(map(r => r.value)));
  }

  private getAccountSearch(): ISearchService {
    return {
      items: this.accountSearch$.asReadonly(),
      search: async (v: string) => {
        await this.doAccountSearch(v);
        return Promise.resolve();
      },
    };
  }

  private async doAccountSearch(v: string) {
    const con = new Condition().isActive();
    if (v && v.length > 0) {
      con.where("name", Comparator.StartsWith, v);
    }
    const next = await lastValueFrom(this.xrmService.query(this.localPrototype, con, "name"));
    this.accountSearch$.set(next.value);
  }
}