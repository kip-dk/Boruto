import { NgClass } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { BorutoXrmUIModule } from 'boruto-xrmui';

@Component({
  selector: 'app-bo',
  templateUrl: './bo.component.html',
  styleUrl: './bo.component.scss',
  imports:[BorutoXrmUIModule]
})
export class BoComponent {
}
