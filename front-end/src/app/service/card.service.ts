import { HttpClient, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, filter, map, Observable, throwError } from 'rxjs';
import { Card, CARDS } from '../models/card';


@Injectable({
  providedIn: 'root'
})
export class CardService {

  constructor(private http: HttpClient) { }

   getCards(): Observable<HttpResponse<Card[]>> {
    return this.http.get<any>('https://raw.githubusercontent.com/adoringlamarr/Example-Apis/main/data/card.json', {observe: 'response'})
    .pipe(
      filter((x => {
        return x.body.access ? x.body.access.toString().toLowerCase() === "everyone" : false;
      })),
    );
  } 
}

  /* getCards(): Observable<any[]>{
    return this.http.get<any[]>(
      'https://raw.githubusercontent.com/adoringlamarr/Example-Apis/main/data/cards.json', 
      {responseType: 'json'}
    ).pipe(
      catchError(this.handleError)
    );
  }  */