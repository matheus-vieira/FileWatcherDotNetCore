import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data-sw',
  templateUrl: './fetch-data-sw.component.html'
})
export class FetchDataSwComponent {
  public people: PeopleSW[];

  constructor(http: HttpClient) {
    http.get<PeopleSW[]>('https://swapi.co/api/people/1/')
      .subscribe(result => {
        this.people = result;
      }, error => console.error(error));
  }
}

interface PeopleSW {
  name: string;
  height: number;
  mass: number;
  hair_color: string;
}
