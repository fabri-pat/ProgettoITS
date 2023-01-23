import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class LoginRegistrationService {

  constructor(private httpClient: HttpClient) { }

  login(username: String, password: String) {
    var body = JSON.stringify({ 'username': username, 'password': password });

    const header = new HttpHeaders().set('Content-Type', 'application/json');

    const httpLoginReq = this.httpClient.post<any>("https://localhost:7155/api/v1/User/login", body, { headers: header, observe: "body", responseType: "json", withCredentials: true });

    httpLoginReq.subscribe((data: any) => {
      const user: User = new User(data.user.id, data.user.name, data.user.surname, data.user.username);
      localStorage.setItem('user', JSON.stringify(user));
      console.log(data.message);
    });
  }

  signup(name: String, surname: String, username: String, email: String, password: String) {
    var body = JSON.stringify({
      'name': name,
      'surname': surname,
      'username': username,
      'email': email,
      'password': password
    });

    const header = new HttpHeaders().set('Content-Type', 'application/json');

    const httpLoginReq = this.httpClient.post("https://localhost:7155/api/v1/User/register", body, { headers: header, observe: "response", withCredentials: true });

    httpLoginReq.subscribe();
  }
}
