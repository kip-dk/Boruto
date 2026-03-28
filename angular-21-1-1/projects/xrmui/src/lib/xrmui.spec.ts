import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Xrmui } from './xrmui';

describe('Xrmui', () => {
  let component: Xrmui;
  let fixture: ComponentFixture<Xrmui>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Xrmui]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Xrmui);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
