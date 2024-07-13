import { IDisplayCity } from './../../DTOs/DisplayDTOs/IDisplayCity';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CityService } from '../../Services/city.service';
import { Status } from '../../Enums/Status';
import { GenericService } from '../../Services/generic.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-city-list',
  standalone: true,
  imports: [CommonModule,FormsModule,RouterModule],
  templateUrl: './city-list.component.html',
  styleUrl: './city-list.component.css'
})
export class CityListComponent implements OnInit, OnDestroy{

  baseURL:string="http://localhost:5241/api/";

    data:IDisplayCity[]=[];
    governorates:IGovernorate[]=[];
    selectedEntries = 8;
    searchTerm = '';
    currentPage = 1;
    totalPages = 0;
    startIndex = 0;
    endIndex = 0;
    filteredData: IDisplayCity[]=[];
    pagedData: IDisplayCity[]=[];

    cSub:any;
    gSub:any;

    constructor(
      private cityService:CityService,
      private router:Router,
      private governorateService:GenericService<IGovernorate,IGovernorateInsert,IGovernorateUpdate>
    ){}

  ngOnDestroy(): void {
    if (this.cSub != undefined) {
      this.cSub.unsubscribe();
    }
    if (this.gSub != undefined) {
      this.gSub.unsubscribe();
    }
  }

    ngOnInit(): void {
      this.loadCities();
    }

    loadCities(): void {
      this.cSub = this.cityService.GetAll(this.baseURL+"Cities").subscribe({
        next:(cities) => {

          this.data = cities;
          this.updateTable();
          this.gSub = this.governorateService.GetAll(this.baseURL+"governorate").subscribe({
            next:(governorates) => {
              this.governorates = governorates;
              this.updateTable();
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
        });
        },
        error:(error) => {
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
      });
    }

    onEntriesChange(): void {
      this.currentPage = 1;
      this.updateTable();
    }

    onSearchChange(): void {
      this.currentPage = 1;
      this.updateTable();
    }

    previousPage(): void {
      if (this.currentPage > 1) {
        this.currentPage--;
        this.updateTable();
      }
    }

    nextPage(): void {
      if (this.currentPage < this.totalPages) {
        this.currentPage++;
        this.updateTable();
      }
    }

    goToPage(page: number): void {
      if (page >= 1 && page <= this.totalPages) {
        this.currentPage = page;
        this.updateTable();
      }
    }

    updateTable(): void {
      let filteredData = this.data;
      if (this.searchTerm) {
        filteredData = filteredData.filter(
          (row) =>
            row.name.toLowerCase().includes(this.searchTerm.toLowerCase())||
          this.getStatusText(row.status).toLowerCase().includes(this.searchTerm.toLowerCase())
        );
      }

      this.filteredData = filteredData;
      this.totalPages = Math.ceil(
        this.filteredData.length / this.selectedEntries
      );
      this.startIndex = (this.currentPage - 1) * this.selectedEntries;
      this.endIndex = Math.min(
        this.startIndex + this.selectedEntries,
        this.filteredData.length
      );
      this.pagedData = this.filteredData.slice(this.startIndex, this.endIndex);
    }
    getStatusText(status: Status): string {
      return Status[status];
    }
    getRowIndex(index: number): number {
      return this.startIndex + index + 1;
    }
    getGovernorateName(governorateId:number) {
      const governorate = this.governorates.find(gov => gov.id === governorateId);
      return governorate ? governorate.name : 'Unknown';
    }
}
