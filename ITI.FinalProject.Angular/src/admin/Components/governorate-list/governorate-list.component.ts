import { Component, OnDestroy, OnInit } from '@angular/core';
import { IDisplayMerchant } from '../../DTOs/DisplayDTOs/IDisplayMerchant';
import { GenericService } from '../../Services/generic.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import { Status } from '../../Enums/Status';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-governorate-list',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './governorate-list.component.html',
  styleUrl: './governorate-list.component.css'
})
export class GovernorateListComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/";
  data: IGovernorate[] = [];
  selectedEntries = 8;
  searchTerm = '';
  currentPage = 1;
  totalPages = 0;
  totalCount = 0;
  
  
  
  

  gSub:any;

  constructor(
    private governorateService: GenericService<IGovernorate, IGovernorateInsert, IGovernorateUpdate>
  ) {
    
  }

  ngOnInit(): void {
    this.loadGovernorates();
  }

  ngOnDestroy(): void {
    if (this.gSub) {
      this.gSub.unsubscribe();
    }
  }

  loadGovernorates(url:string = `${this.baseURL}GovernoratePage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}&name=${this.searchTerm}`): void {
    this.gSub = this.governorateService.GetPage(url).subscribe({
      next: (data:any) =>{
        console.log(data);

        this.data = data.list;
        this.totalPages = data.totalPages;
        this.totalCount = data.totalCount
        
      },
      error: error => {
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
      }
    })
  }

  onEntriesChange(): void {
    this.currentPage = 1;
    
    this.loadGovernorates();
  }

  onSearchChange(): void {
    this.currentPage = 1;
    
    this.loadGovernorates();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      
      this.loadGovernorates();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      
      this.loadGovernorates();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      
      this.loadGovernorates();
    }
  }

  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  

  
  
  
  
  
  
  
  
  
  
  

  getStatusText(status: Status): string {
    return Status[status];
  }

  
  
  
}
