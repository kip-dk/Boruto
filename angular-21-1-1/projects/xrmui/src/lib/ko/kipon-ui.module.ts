import { NgModule } from '@angular/core';
import { KoHorizontalScrollComponent } from './panels/kohorizontalscroll/kohorizontalscroll.component';
import { KoHorizontalSplitComponent } from './panels/kohorizontalsplit/kohorizontalsplit.component';
import { KoMainComponent } from './panels/komain/komain.component';
import { KoTitlePanelComponent } from './panels/kotitlepanel/kotitlepanel.component';
import { KoVerticalScrollComponent } from './panels/koverticalscroll/koverticalscroll.component';
import { KoVerticalSplitComponent } from './panels/koverticalsplit/koverticalsplit.component';
import { KoViewComponent } from './panels/koview/koview.component';
import { KoExpansionPanelComponent } from './panels/koexpansionpanel/koexpansionpanel.component';
import { KoTablePanelComponent } from './panels/kotablepanel/kotablepanel.component';
import { KoButtonGroupComponent } from './forms/kobuttongroup/kobuttongroup.component';
import { KoDecimalDirective } from './forms/kodecimal.directive';
import { KoFocusDirective } from './forms/kofocus.directive';
import { KoAlertComponent } from './message/koalert/koaleret.component';
import { KoHeaderComponent } from './message/koheader/koheader.component';
import { KoOverDirective } from './navigation/koover.directive';
import { KoTopMenuComponent } from './navigation/kotopmenu/kotopmenu.component';
import { KoLeftMenuComponent } from './navigation/koleftmenu/koleftmenu.component';
import { KoWriteDirective } from './message/kowrite.directive';


@NgModule({
  imports: [
    KoHorizontalScrollComponent,
    KoHorizontalSplitComponent,
    KoMainComponent,
    KoTitlePanelComponent,
    KoVerticalScrollComponent,
    KoVerticalSplitComponent,
    KoViewComponent,
    KoExpansionPanelComponent,
    KoTablePanelComponent,
    KoButtonGroupComponent,
    KoDecimalDirective,
    KoFocusDirective,
    KoAlertComponent,
    KoHeaderComponent,
    KoOverDirective,
    KoTopMenuComponent,
    KoLeftMenuComponent,
    KoWriteDirective
  ],
  exports: [
    KoHorizontalScrollComponent,
    KoHorizontalSplitComponent,
    KoMainComponent,
    KoTitlePanelComponent,
    KoVerticalScrollComponent,
    KoVerticalSplitComponent,
    KoViewComponent,
    KoExpansionPanelComponent,
    KoTablePanelComponent,
    KoButtonGroupComponent,
    KoDecimalDirective,
    KoFocusDirective,
    KoAlertComponent,
    KoHeaderComponent,
    KoOverDirective,
    KoTopMenuComponent,
    KoLeftMenuComponent,
    KoWriteDirective
  ]
})
export class KiponUiModule { }
