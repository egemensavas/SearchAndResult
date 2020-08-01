import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private REST_API_SERVER;

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) { this.REST_API_SERVER = baseUrl }

  public sendGetRequest(method : string) {

    return this.httpClient.get(this.REST_API_SERVER + method);
  }
}
