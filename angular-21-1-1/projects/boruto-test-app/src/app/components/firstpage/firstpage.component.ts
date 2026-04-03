import { Component, inject, signal } from '@angular/core';
import { XrmContextService } from 'xrm';

@Component({
  selector: 'app-firstpage',
  templateUrl: './firstpage.component.html',
  styleUrl: './firstpage.component.scss',
  imports: []
})
export class FirstpageComponent {
  form: XrmContextService = inject(XrmContextService);

}
