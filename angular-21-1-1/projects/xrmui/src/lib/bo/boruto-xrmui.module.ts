import { NgModule } from '@angular/core';
import { XrmuiInfo } from 'xrmui';
import { XrmuiLayout } from './layout/layout.xrmui';

@NgModule({
  imports: [
    XrmuiInfo,
    XrmuiLayout
  ],
  exports: [
    XrmuiInfo,
    XrmuiLayout
  ]
})
export class BorutoXrmUIModule { }
