import { NgClass } from '@angular/common';
import { Component, input,  model } from '@angular/core';
import { IMenu } from '../api/imenu.interface';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatBadge } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';

@Component({
    selector: 'xrmui-tabs',
    templateUrl: './tabs.xrmui.html',
    styleUrl: './tabs.xrmui.scss',
    imports: [NgClass,MatIcon,MatBadge,MatButtonModule, MatMenuModule, MatIconModule],
    providers: []
})
export class XrmuiTabs {
    tabs = input.required<IMenu[]>();
    burgers = input<IMenu[]>([]);
    current = model.required<IMenu>();

    constructor() {
    }
}
