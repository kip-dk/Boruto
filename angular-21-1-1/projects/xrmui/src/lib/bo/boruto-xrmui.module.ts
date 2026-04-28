import { NgModule } from '@angular/core';
import { XrmuiInfo } from './info/info.xrmui';
import { XrmuiPanel } from './panel/panel.xrmui'
import { XrmuiLayout } from './layout/layout.xrmui';
import { XrmuiIcon } from './icon/icon.xrmui';
import { XrmuiInput } from './input/input.xrmui';

@NgModule({
  imports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput
  ],
  exports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput
  ]
})
export class BorutoXrmUIModule { }
