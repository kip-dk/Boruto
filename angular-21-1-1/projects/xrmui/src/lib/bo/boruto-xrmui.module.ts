import { NgModule } from '@angular/core';
import { XrmuiInfo } from './info/info.xrmui';
import { XrmuiPanel } from './panel/panel.xrmui'
import { XrmuiLayout } from './layout/layout.xrmui';
import { XrmuiIcon } from './icon/icon.xrmui';
import { XrmuiInput } from './input/input.xrmui';
import { XrmuiRibbon } from './ribbon/ribbon.xrmui';
import { XrmuiSpinner } from './spinner/spinner.xrmui';
import { XrmuiButton } from './button/button.xrmui';
import { XrmuiDevider } from './devider/devider.xrmui';
import { XrmuiTabs } from './tabs/tabs.xrmui';
import { ChangeScopeDirective } from './changedscope/changedscope.directive';
import { FormScopeDirective } from './form/formscope.directive';
import { XrmuiSection } from './section/section.xrmui';

@NgModule({
  imports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput,
    XrmuiRibbon,
    XrmuiSpinner,
    XrmuiButton,
    XrmuiDevider,
    XrmuiTabs,
    XrmuiSection,
    ChangeScopeDirective,
    FormScopeDirective,
  ],
  exports: [
    XrmuiInfo,
    XrmuiLayout,
    XrmuiPanel,
    XrmuiIcon,
    XrmuiInput,
    XrmuiRibbon,
    XrmuiSpinner,
    XrmuiButton,
    XrmuiDevider,
    XrmuiTabs,
    XrmuiSection,
    ChangeScopeDirective,
    FormScopeDirective
  ]
})
export class BorutoXrmUIModule { }
