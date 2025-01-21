import { ComponentFixture, TestBed } from '@angular/core/testing';

import { XrmuiComponent } from './xrmui.component';

describe('XrmuiComponent', () => {
  let component: XrmuiComponent;
  let fixture: ComponentFixture<XrmuiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [XrmuiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(XrmuiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
