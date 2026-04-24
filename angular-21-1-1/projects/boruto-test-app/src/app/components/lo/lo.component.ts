import { NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { BorutoXrmUIModule } from 'boruto-xrmui';

@Component({
  selector: 'app-lo',
  templateUrl: './lo.component.html',
  styleUrl: './lo.component.scss',
  imports:[BorutoXrmUIModule]
})
export class LoComponent {
}
