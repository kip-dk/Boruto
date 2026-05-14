import { NgModule } from '@angular/core';
import { XrmuiInfo } from './info/info.xrmui';
import { XrmuiPanel } from './panel/panel.xrmui'
import { XrmuiLayout } from './layout/layout.xrmui';
import { XrmuiIcon } from './icon/icon.xrmui';
import { XrmuiInput } from './input/input.xrmui';
import { XrmuiRibbon } from './ribbon/ribbon.xrmui';
import { XrmuiSpinner } from './spinner/spinner.xrmui';
import { XrmuiButton } from './button/button.xrmui';

@NgModule({
  imports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput,
    XrmuiRibbon,
    XrmuiSpinner,
    XrmuiButton
  ],
  exports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput,
    XrmuiRibbon,
    XrmuiSpinner,
    XrmuiButton
  ]
})
export class BorutoXrmUIModule { }
