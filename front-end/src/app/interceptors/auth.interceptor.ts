import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';
import { map, Observable, tap } from 'rxjs';


@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor() { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let ok: any;

    return next.handle(request)
      .pipe(
        tap({
          next: (event) => {
            if (event instanceof HttpResponse) {
              ok = event.body;
            }
          },
          error: (error: HttpErrorResponse) => (ok = error.error.message),
        }),
      )
  }
}