import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { LoginRegistrationFormComponent } from '../login-registration-form/login-registration-form.component';

@Component({
  selector: 'app-trial-dynamic-component',
  template: `
    <h1>Hello angular 14</h1>
    <h2>Michele giove</h2>
    `,
  styleUrls: ['./trial-dynamic-component.component.css']
})
export class TrialDynamicComponentComponent implements OnInit {

  constructor(
    private viewContainerRef: ViewContainerRef
  ) { }

  ngOnInit(): void {
    this.viewContainerRef.createComponent(LoginRegistrationFormComponent);
  }
}
