import { TestBed } from '@angular/core/testing';

import { XrmuiService } from './xrmui.service';

describe('XrmuiService', () => {
  let service: XrmuiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(XrmuiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
