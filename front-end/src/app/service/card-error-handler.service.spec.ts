import { TestBed } from '@angular/core/testing';

import { CardErrorHandlerService } from './card-error-handler.service';

describe('CardErrorHandlerService', () => {
  let service: CardErrorHandlerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CardErrorHandlerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
