import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-ViewerSHB',
  templateUrl: './ViewerSHB.component.html'
})
export class ViewerSHBComponent {
  public Adverts: AdvertModel[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<AdvertModel[]>(baseUrl + 'API').subscribe(result => {
      this.Adverts = result;
      console.log(result);
    }, error => console.error(error));
  }
}

interface AdvertModel {
  AdvertLink: string;
  Description: string;
  ThumbnailLink: string;
  Location: string;
  AdvertDate: string;
  Price: string;
}
