import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-ViewerSHB',
  templateUrl: './ViewerSHB.component.html'
})
export class ViewerSHBComponent {
  public viewerdata: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<string>(baseUrl + 'API').subscribe(result => {
      this.viewerdata = result;
      console.log(result);
    }, error => console.error(error));
  }
}


