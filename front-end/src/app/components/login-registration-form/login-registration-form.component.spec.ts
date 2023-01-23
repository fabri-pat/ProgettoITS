import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginRegistrationFormComponent } from './login-registration-form.component';

describe('RegistrationFormComponent', () => {
  let component: LoginRegistrationFormComponent;
  let fixture: ComponentFixture<LoginRegistrationFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LoginRegistrationFormComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginRegistrationFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
