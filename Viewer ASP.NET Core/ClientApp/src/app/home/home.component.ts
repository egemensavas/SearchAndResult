import { Component, ElementRef, ViewChild  } from '@angular/core';
import { DataService } from '../data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ['div a { cursor: pointer; }']
})
export class HomeComponent implements OnInit {
  public Adverts: AdvertModel[];
  public SearchMasters: SearchMasterModel[];
  public filteredAdverts: AdvertModel[];
  sortType: string;
  sortReverse: boolean = false;

  constructor(private dataService: DataService) { }

  ngOnInit() {
    this.dataService.sendGetRequest("API/FillDataToScreen?SearchMasterID=0").subscribe((data: any[]) => {
      console.log(data);
      this.Adverts = data;
      this.filteredAdverts = this.Adverts;
      this.sortAdverts("date_sort");
    });
    this.dataService.sendGetRequest("API/FillSearchComboData").subscribe((data: any[]) => {
      console.log(data);
      this.SearchMasters = data;
      this.sortSearchMasters("description");
    })
  }

  sortAdverts(property) {
    this.sortType = property;
    this.sortReverse = !this.sortReverse;
    this.filteredAdverts.sort(this.dynamicSort(property));
  }

  sortSearchMasters(property) {
    this.sortType = property;
    this.sortReverse = !this.sortReverse;
    this.SearchMasters.sort(this.dynamicSort(property));
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
    this.sortAdverts("date_sort");
  }

  filterAdverts(selectedID: number, search: string) {
    if (selectedID > 0)
      this.filteredAdverts = this.Adverts.filter(o => o.searchMasterID == selectedID)
    if (search != "")
      this.filteredAdverts = this.Adverts.filter(o =>
        Object.keys(o).some(k => {
          if (typeof o[k] === 'string')
            return o[k].toLowerCase().includes(search.toLowerCase())
        })
      )
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

interface SearchMasterModel {
  ID: number;
  Description: string;
  Notes: string;
  RecordCount: number;
}

interface OnInit {
  ngOnInit(): void
}
