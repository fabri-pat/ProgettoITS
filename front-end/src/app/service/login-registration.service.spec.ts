import { TestBed } from '@angular/core/testing';

import { LoginRegistrationService } from './login-registration.service';

describe('RegistrationService', () => {
  let service: LoginRegistrationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoginRegistrationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
