import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';
import { Xrmui } from 'xrmui'

@Component({
  selector: 'app-firstpage',
  templateUrl: './firstpage.component.html',
  styleUrl: './firstpage.component.scss',
  imports: [Xrmui]
})
export class FirstpageComponent {
  form: XrmContextService = inject(XrmContextService);

}
