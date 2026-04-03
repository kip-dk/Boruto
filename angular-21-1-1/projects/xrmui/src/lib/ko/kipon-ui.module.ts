import { NgModule } from '@angular/core';
import { KoAlertComponent, KoButtonGroupComponent, KoDecimalDirective, KoExpansionPanelComponent, KoFocusDirective, KoHeaderComponent, KoHorizontalScrollComponent, KoHorizontalSplitComponent, KoLeftMenuComponent, KoMainComponent, KoOverDirective, KoTablePanelComponent, KoTitlePanelComponent, KoTopMenuComponent, KoVerticalScrollComponent, KoVerticalSplitComponent, KoViewComponent, KoWriteDirective } from 'xrmui';

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
