import { Component, Input } from '@angular/core';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'my-app';

  @Input()
  user!: User;

  isLogged() :boolean {
    var userString = localStorage.getItem('user');

    if (userString != null){
      this.user = (JSON.parse(userString) as User)
      return true;
    }
    return false;
  }
}
