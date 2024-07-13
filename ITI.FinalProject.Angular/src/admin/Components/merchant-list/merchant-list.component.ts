import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { IDisplayMerchant } from '../../DTOs/DisplayDTOs/IDisplayMerchant';
import { MerchantService } from '../../Services/merchant.service';
import { Status } from '../../Enums/Status';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-merchant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './merchant-list.component.html',
  styleUrls: ['./merchant-list.component.css'],
})
export class MerchantListComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/"

  data: IDisplayMerchant[] = [];
  selectedEntries = 7;
  searchTerm = '';
  currentPage = 1;
  totalPages = 0;
  totalCount = 0;
  startIndex = 0;
  endIndex = 0;
  filteredData: IDisplayMerchant[] = [];
  pagedData: IDisplayMerchant[] = [];
  private destroy$ = new Subject<void>();
  url: string = `${this.baseURL}MerchantPage?pageNumber=1&pageSize=${this.selectedEntries}`;

  constructor(private merchantService: MerchantService) {}

  ngOnInit(): void {
    this.loadMerchants();
  }

  loadMerchants(): void {
    this.merchantService
      .GetPage(this.url)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: any) => {
          console.log(response);
          if (response && response.list && Array.isArray(response.list)) {
            this.data = response.list;
            this.totalPages =
              response.totalPages ||
              Math.ceil(response.totalCount / this.selectedEntries);
            this.totalCount = response.totalCount;
            this.startIndex = (this.currentPage - 1) * this.selectedEntries + 1;
            this.endIndex = Math.min(
              this.currentPage * this.selectedEntries,
              this.totalCount
            );
          } else {
            console.error('Unexpected API response format:', response);
          }
          this.updateTable();
        },
        error: (error) => {
          if (error.status == 401) {
            Swal.fire({
              icon: "error",
              title: "Error",
              text: "Unauthorized",
            })
          }
          else if (error.error.message) {          
            Swal.fire({
              icon: "error",
              title: "Error",
              text: error.error.message,
            })
          }else{
            Swal.fire({
              icon: "error",
              title: "Error",
              text: "Something went wrong, please try again later",
            })
          }
        },
      });
  }

  onEntriesChange(): void {
    this.currentPage = 1;
    this.url = `${this.baseURL}MerchantPage?pageNumber=1&pageSize=${this.selectedEntries}`;
    this.loadMerchants();
  }

  onSearchChange(): void {
    this.currentPage = 1;
    this.loadMerchants();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.url = `${this.baseURL}MerchantPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadMerchants();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.url = `${this.baseURL}MerchantPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadMerchants();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.url = `${this.baseURL}MerchantPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}`;
      this.loadMerchants();
    }
  }

  updateTable(): void {
    this.filteredData = this.data || [];
    this.pagedData = this.filteredData;
  }

  getStatusText(status: Status): string {
    return Status[status];
  }

  getRowIndex(index: number): number {
    return this.startIndex + index + 1;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
