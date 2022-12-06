export class Card {
   title?: string;
   description?: string;
   button?: string;
}

 export const CARDS: Card[] = [
   {
      title: "First card",
      description: "This is my first card, be patient!"
   },
   {
      title: "Second Card"
   },
   {
      title: "Third Card",
      description: "Oh yes we have added also a a button!",
      button: "Click and die"
   },
   {
      title: "Fourth Card",
      description: "Another card...uff!",
      button: "Click me!"
   }
];