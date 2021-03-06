import { Component } from '@angular/core';
import { DataService } from '../data.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styles: ['div a { cursor: pointer; }']
})
export class HomeComponent implements OnInit {
  public Adverts: AdvertModel[];
  public filteredAdverts: AdvertModel[];
  public paginatedAdverts: AdvertModel[];
  public SearchList: SearchModel[];
  public paginateBase = [] as Array<PaginateModel>
  public paginateShow = [] as Array<PaginateModel>
  sortType: string;
  sortReverse: boolean = true;
  chosenSearchMasterID: number;
  chosenTextInput: string = "";
  public pageSize: number = 8;
  public paginationSize = 6;
  public selectedPage: number = 1;
  public selectedPageString: string = "1";

  constructor(private dataService: DataService) { }

  ngOnInit() {
    this.fillSearchMasterData()
    this.fillAdvertData()
  }

  fillPagination() {
    this.paginateBase = []
    let pagecount: number = Math.ceil(this.filteredAdverts.length / this.pageSize)
    var n: number = 1
    while (n <= pagecount) {
      var x: PaginateModel = {
        PageNumber: n.toString()
      }
      this.paginateBase.push(x)
      n++
    }
  }

  fillPaginationShow(input) {
    if (input == "<<") {
      this.fillPaginationShow(1)
      return
    }
    if (input == ">>") {
      this.fillPaginationShow(this.paginateBase.length)
      return
    }
    if (input == "<") {
      if (this.selectedPage > 1)
        this.fillPaginationShow(this.selectedPage - 1)
      return
    }

    if (input == ">") {
      if (this.selectedPage < this.paginateBase.length)
        this.fillPaginationShow(this.selectedPage + 1)
      return
    }
    this.selectedPage = +input;
    this.selectedPageString = this.selectedPage.toString()
    this.paginateShow = []
    var x: PaginateModel
    x = { PageNumber: "<<" }
    this.paginateShow.push(x)
    x = { PageNumber: "<" }
    this.paginateShow.push(x)

    let lastPage: number = this.paginationSize
    if (this.paginateBase.length < lastPage)
      lastPage = this.paginateBase.length

    if (this.selectedPage - 2 > 0 && this.selectedPage + 2 <= this.paginateBase.length) {
      var n: number = -2
      while (n <= lastPage - n) {
        x = { PageNumber: (this.selectedPage + n).toString() }
        this.paginateShow.push(x)
        n++
      }
    }

    if (this.selectedPage - 2 <= 0) {
      var n: number = 1
      while (n <= lastPage) {
        x = { PageNumber: (n).toString() }
        this.paginateShow.push(x)
        n++
      }
    }
    else if (this.selectedPage + 2 > this.paginateBase.length) {
      var n: number = lastPage
      while (n > 0) {
        x = { PageNumber: (this.paginateBase.length - n + 1).toString() }
        this.paginateShow.push(x)
        n--
      }
    }

    x = { PageNumber: ">" }
    this.paginateShow.push(x)
    x = { PageNumber: ">>" }
    this.paginateShow.push(x)

    this.doPaginate()
  }

  fillSearchMasterData() {
    this.dataService.sendGetRequest("API/FillSearchComboData").subscribe((data: any[]) => {
      console.log(data);
      this.SearchList = data;
      this.sortSearchList("description");
    })
  }

  fillAdvertData() {
    this.dataService.sendGetRequest("API/FillDataToScreen").subscribe((data: any[]) => {
      console.log(data);
      this.Adverts = data;
      this.filteredAdverts = this.Adverts;
      this.fillPagination()
      this.fillPaginationShow(this.selectedPage)
      this.sortAdverts("date_sort", false);
      this.doPaginate()
    })
  }

  doPaginate() {
    let startRecord: number = this.pageSize * (this.selectedPage - 1)
    this.paginatedAdverts = this.filteredAdverts.slice(startRecord, startRecord + this.pageSize)
  }

  sortAdverts(property, reverse: boolean) {
    this.sortType = property;
    if (reverse)
      this.sortReverse = !this.sortReverse;
    this.filteredAdverts.sort(this.dynamicSort(property));
    this.doPaginate()
  }

  sortSearchList(property) {
    this.sortType = property;
    this.sortReverse = !this.sortReverse;
    this.SearchList.sort(this.dynamicSort(property));
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

  commonMethod() {
    this.filterAdverts(this.chosenSearchMasterID, this.chosenTextInput)
    this.sortReverse = false
    this.sortAdverts("date_sort", false)
    this.fillPagination()
    this.fillPaginationShow(1)
    this.doPaginate()
  }

  filterAdverts(selectedID: number, search: string) {
    this.filteredAdverts = this.Adverts
    if (selectedID > 0)
      this.filteredAdverts = this.Adverts.filter(o => o.searchMasterID == selectedID)
    if (search != "")
      this.filteredAdverts = this.filteredAdverts.filter(o =>
        Object.keys(o).some(k => {
          if (typeof o[k] === 'string')
            return o[k].toLowerCase().includes(search.toLowerCase())
        })
      )
  }

  keyUpEvent(input: string) {
    this.chosenTextInput = input
    this.commonMethod()
  }

  onChangeEvent(ev) {
    this.chosenSearchMasterID = ev.target.value
    this.commonMethod()
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

interface SearchModel {
  SearchID: number;
  Description: string;
}

interface OnInit {
  ngOnInit(): void
}

interface PaginateModel {
  PageNumber: string
}
