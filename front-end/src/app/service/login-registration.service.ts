import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService {

  constructor(private httpClient: HttpClient) { }

  login(username: String, password: String) {
    var body = JSON.stringify({ 'username': username, 'password': password });

    const header = new HttpHeaders().set('Content-Type', 'application/json');

    const httpLoginReq = this.httpClient.post("https://localhost:7155/api/v1/User/login", body, { headers: header, observe: "response", responseType: "text", withCredentials: true });

    httpLoginReq.subscribe();
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
