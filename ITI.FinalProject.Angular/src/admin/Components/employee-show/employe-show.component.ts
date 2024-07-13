import { EmployeeService } from './../../Services/employee.service';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { IEmployee } from '../../DTOs/DisplayDTOs/IEmployee';
import { Status } from '../../Enums/Status';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-employe-show',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './employe-show.component.html',
  styleUrls: ['./employe-show.component.css'],
})
export class EmployeshowComponent implements OnInit, OnDestroy {
  baseURL:string="http://localhost:5241/api/";
  data: IEmployee[] = [];
  selectedEntries = 8;
  searchTerm = '';
  currentPage = 1;
  totalPages = 0;
  startIndex = 0;
  endIndex = 0;
  filteredData: IEmployee[] = [];
  pagedData: IEmployee[] = [];

  eSub:any;

  constructor(
    private employeeService: EmployeeService,
    private router: Router
  ) {}

  ngOnDestroy(): void {
    if (this.eSub != undefined) {
      this.eSub.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.eSub = this.employeeService.GetAll(`${this.baseURL}Employees`).subscribe(
      {next:(employees: IEmployee[]) => {
        this.data = employees;
        this.updateTable();
      },
      error:(error: any) => {
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
      }}
    );
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
          row.userName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
          row.email.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
          row.phoneNumber.includes(this.searchTerm) ||
          
          this.getStatusText(row.status).toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    this.filteredData = filteredData;
    this.totalPages = Math.ceil(this.filteredData.length / this.selectedEntries);
    this.startIndex = (this.currentPage - 1) * this.selectedEntries;
    this.endIndex = Math.min(this.startIndex + this.selectedEntries, this.filteredData.length);
    this.pagedData = this.filteredData.slice(this.startIndex, this.endIndex);
  }


  getStatusText(status: Status): string {
    return Status[status];
  }
  getRowIndex(index: number): number {
    return this.startIndex + index + 1;
  }
}


