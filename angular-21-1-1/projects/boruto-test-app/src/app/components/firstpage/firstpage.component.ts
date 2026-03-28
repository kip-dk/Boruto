import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';

@Component({
  selector: 'app-root',
  templateUrl: './firstpage.component.html',
  styleUrl: './firstpage.component.scss'
})
export class FirstpageComponent {
  private xrmContext: XrmContextService = inject(XrmContextService);
}
