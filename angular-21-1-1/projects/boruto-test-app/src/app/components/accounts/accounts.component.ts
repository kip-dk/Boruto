import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { BorutoXrmUIModule } from 'boruto-xrmui';
import { AccountService } from '../../services/account.service';
import { Account } from '../../entities/account.entity';
import { OwnershipcodeOptions } from '../../models/ownership.enum';
import { GroupOptions } from '../../models/group.enum';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrl: './accounts.component.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports:[BorutoXrmUIModule]
})
export class AccountsComponent {
    readonly accountService = inject(AccountService);

    accounts = signal<Account[]>([]);
    account = signal<Account | null>(null);

    ownershipOptions = OwnershipcodeOptions;
    groupOptions = GroupOptions;

    constructor() {
        this.init();
    }

    private async init() {
        const next = await this.accountService.active();
        this.accounts.set(next);
        if (next.length > 0) {
            this.account.set(next[0]);
        }
    }

    async save(a: Account) {
        await this.accountService.update(a);
    }
}
