export class FilterModel {
  public sortType: string;
  public fromDate: Date;
  public toDate: Date;
  public pageNumber: number;
  public pageSize: number;

  constructor() {
    this.pageSize = 15;
    this.pageNumber = 1;
    this.sortType = 'desc'
  }

}