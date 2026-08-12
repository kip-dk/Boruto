import { NgClass } from '@angular/common';
import { Component, effect, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { BorutoXrmUIModule } from 'boruto-xrmui';

@Component({
  selector: 'app-lo',
  templateUrl: './lo.component.html',
  styleUrl: './lo.component.scss',
  changeDetection: ChangeDetectionStrategy.Eager,
  imports:[BorutoXrmUIModule]
})
export class LoComponent {

  myInput = signal<string>('');;

  constructor() {
    effect(() => {
      const mi = this.myInput();
    });
  }

  forcevalue() {
    this.myInput.set("something totally different");
  }
}
