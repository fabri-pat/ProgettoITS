import { HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardErrorHandlerService<T> {

  t!: T;

  constructor() {  }

  public handleError(error: HttpErrorResponse, defaultObject?:T) {
    if(error.status === 0) {
      console.error("Client or network error.", error.error);
    } else {
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
      /* return new Observable<HttpResponse<Card[]>>( x => x.next(new HttpResponse<Card[]>({body: CARDS}))); */
    }
    if (defaultObject) {
      return new Observable<HttpResponse<T>>(x => x.next(new HttpResponse<T>({body: defaultObject})));
    }
    else {
      return throwError(() => new Error('Something bad happened; please try again later.'));
    }
  }
}
