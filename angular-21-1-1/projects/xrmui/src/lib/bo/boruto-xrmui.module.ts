import { NgModule } from '@angular/core';
import { XrmuiIcon, XrmuiInfo, XrmuiPanel } from 'xrmui';
import { XrmuiLayout } from './layout/layout.xrmui';

@NgModule({
  imports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon
  ],
  exports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon
  ]
})
export class BorutoXrmUIModule { }
