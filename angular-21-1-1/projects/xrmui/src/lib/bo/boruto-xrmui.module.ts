import { NgModule } from '@angular/core';
import { XrmuiInfo } from './info/info.xrmui';
import { XrmuiPanel } from './panel/panel.xrmui'
import { XrmuiLayout } from './layout/layout.xrmui';
import { XrmuiIcon } from './icon/icon.xrmui';

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
