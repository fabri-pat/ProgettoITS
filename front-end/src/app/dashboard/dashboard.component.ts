
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Card, CARDS } from '../models/card';
import { CardErrorHandlerService } from '../service/card-error-handler.service';
import { CardService } from '../service/card.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  cards! :Card[];
  title?: string = "My dashboard";
  cardFromUrl?: any[];

  constructor(private cardService: CardService, private cardErrorHandler: CardErrorHandlerService<Card[]>) {

  }

  ngOnInit(): void {
    this.cardService
    .getCards()
    .subscribe(
      {next: data => {
        if(data.ok){
          this.cards = data.body!
          .map((x: any) => {
            return {
              title: x.title,
              description: x.description,
              button: x.icon ? x.icon : x.button
            } as Card;
          });  
        }
      },
      error: (e :HttpErrorResponse) => {
        this.cardErrorHandler.handleError(e, CARDS).subscribe({ next: data => this.cards = data.body!});
      }
    }
    )
  }

  public get CardService(){
    return this.cardService;
  }

  public set CardService(cardService: CardService){
    this.cardService = cardService;
  }
}


/* ngOnInit(): void {
  this.cardService
  .getCards()
  .subscribe(
    data => {
      this.cards = data
      .filter(x =>  {
        return x.access ? x.access.toString().toLowerCase() === "everyone" : false;
      })
      .map( (x: any) => {
        return {
          title: x.title,
          description: x.description,
          button: x.icon
        } as Card;
      });
      console.log(this.cards);
    }
  )
} */