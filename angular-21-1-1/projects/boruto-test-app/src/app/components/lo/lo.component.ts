import { NgClass } from '@angular/common';
import { Component, effect, inject, signal } from '@angular/core';
import { BorutoXrmUIModule } from 'boruto-xrmui';

@Component({
  selector: 'app-lo',
  templateUrl: './lo.component.html',
  styleUrl: './lo.component.scss',
  imports:[BorutoXrmUIModule]
})
export class LoComponent {

  myInput = signal<string>('');;

  constructor() {
    effect(() => {
      const mi = this.myInput();
      console.log(mi);
    });
  }

  forcevalue() {
    this.myInput.set("something totally different");
  }
}
