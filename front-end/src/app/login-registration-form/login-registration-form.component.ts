import { Component, OnInit } from '@angular/core';
import { RegistrationService } from '../service/login-registration.service';

@Component({
  selector: 'app-login-registration-form',
  templateUrl: './login-registration-form.component.html',
  styleUrls: ['./login-registration-form.component.css']
})
export class RegistrationFormComponent implements OnInit {

  constructor(private registrationService: RegistrationService) { }

  ngOnInit(): void {
    const signUpButton = <HTMLBodyElement>document.getElementById('signUp');
    const signInButton = <HTMLBodyElement>document.getElementById('signIn');
    const container = <HTMLBodyElement>document.getElementById('container');

    signUpButton.addEventListener('click', () => {
      container.classList.add("right-panel-active");
    });

    signInButton.addEventListener('click', () => {
      container.classList.remove("right-panel-active");
    });
  }

  login() {
    var username = (<HTMLInputElement>document.getElementById("login-username")).value;
    var password = (<HTMLInputElement>document.getElementById("login-password")).value;

    this.registrationService.login(username, password);
  }

  signup() {
    var name = (<HTMLInputElement>document.getElementById("signup-name")).value;
    var surname = (<HTMLInputElement>document.getElementById("signup-surname")).value;
    var username = (<HTMLInputElement>document.getElementById("signup-username")).value;
    var email = (<HTMLInputElement>document.getElementById("signup-email")).value;
    var password = (<HTMLInputElement>document.getElementById("signup-password")).value;

    this.registrationService.signup(name, surname, username, email, password);
  }
}
