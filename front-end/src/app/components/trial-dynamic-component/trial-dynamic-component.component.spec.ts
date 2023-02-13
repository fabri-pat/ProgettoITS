import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrialDynamicComponentComponent } from './trial-dynamic-component.component';

describe('TrialDynamicComponentComponent', () => {
  let component: TrialDynamicComponentComponent;
  let fixture: ComponentFixture<TrialDynamicComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TrialDynamicComponentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrialDynamicComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
