import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ['div a { cursor: pointer; }']
})
export class HomeComponent implements OnInit {
  public Adverts: AdvertModel[];
  public filteredAdverts: AdvertModel[];
  sortType: string;
  sortReverse: boolean = false;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<AdvertModel[]>(baseUrl + 'API/FillDataToScreen?SearchMasterID=0').subscribe(result => {
      this.Adverts = result;
      //console.log(result);
    }, error => console.error(error));
    this.filteredAdverts = this.Adverts;
  }

  ngOnInit() {
    this.filteredAdverts = this.Adverts;
  }

  sortAdverts(property) {
    this.sortType = property;
    this.sortReverse = !this.sortReverse;
    this.filteredAdverts.sort(this.dynamicSort(property));
  }

  dynamicSort(property) {
    let sortOrder = -1;

    if (this.sortReverse) {
      sortOrder = 1;
    }

    return function (a, b) {
      let result = a[property] < b[property] ? -1 : a[property] > b[property] ? 1 : 0;
      return result * sortOrder;
    };
  }

  onChangeEvent(ev) {
    this.filterAdverts(ev.target.value, "");
    this.sortAdverts("price");
  }

  filterAdverts(selectedID: number, search: string) {
    if (selectedID > 0)
      this.filteredAdverts = this.Adverts.filter(o => o.searchMasterID == selectedID)
    if (search != "")
      this.filteredAdverts = this.Adverts.filter(o =>
        Object.keys(o).some(k => {
          if (typeof o[k] === 'string')
            return o[k].toLowerCase().includes(search.toLowerCase());
        })
      );
  }
}

interface AdvertModel {
  AdvertLink: string;
  Description: string;
  ThumbnailLink: string;
  Location: string;
  AdvertDate: string;
  Price: string;
  Size: string;
  Room: string;
  Heating: string;
  Price_sort: number;
  searchMasterID: number;
  Date_sort: number;
}

interface OnInit {
  ngOnInit(): void
}

