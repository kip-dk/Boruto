import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { XrmContextService } from 'boruto-xrmservice';

@Component({
  selector: 'app-firstpage',
  templateUrl: './firstpage.component.html',
  styleUrl: './firstpage.component.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: []
})
export class FirstpageComponent {
  form: XrmContextService = inject(XrmContextService);

}
