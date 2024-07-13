import { Status } from './../../Enums/Status';

import { Component, OnDestroy, OnInit } from '@angular/core';
import { BranchService } from '../../Services/branch.service';
import { GenericService } from '../../Services/generic.service';
import { IBranch } from '../../DTOs/DisplayDTOs/IBranch';
import { IBranchInsert } from '../../DTOs/InsertDTOs/IBranchInsert';
import { IBranchUpdate } from '../../DTOs/UpdateDTOs/IBranchUpdate';
import { CommonModule } from '@angular/common';
import { ICity } from '../../DTOs/DisplayDTOs/ICity';
import { ICityInsert } from '../../DTOs/InsertDTOs/ICityInsert';
import { ICityUpdate } from '../../DTOs/UpdateDTOs/ICityUpdate';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-branch-list',
  standalone: true,
  imports: [CommonModule,RouterLink,FormsModule],
  templateUrl: './branch-list.component.html',
  styleUrl: './branch-list.component.css'
})
export class BranchListComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/"
  branches:IBranch[]=[]
  cities:ICity[]=[]
  selectedEntries:number=8
  searchTerm:string=""
  currentPage:number=1
  totalBranches:number=0
  pagesNum:number=0
  startIndex:number=1
  endIndex:number=1

  cSub:any;
  bSub:any;
  dbSub:any;

  constructor(
    public branchServ:GenericService<IBranch,IBranchInsert,IBranchUpdate>,
    public cityServ: GenericService<ICity, ICityInsert, ICityUpdate>,
  ) {
  }

  ngOnDestroy(): void {
    if (this.cSub != undefined) {
      this.cSub.unsubscribe();
    }
    if (this.bSub != undefined) {
      this.bSub.unsubscribe();
    }
    if (this.dbSub != undefined) {
      this.dbSub.unsubscribe();
    }
  }

  ngOnInit(): void {

    
    this.getBranches();


    
    this.cSub = this.cityServ.GetAll(this.baseURL+"Cities").subscribe({
      next: (value) => {
        console.log(value);

        this.cities = value
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
    })
  }

  getBranches(){
    this.bSub = this.branchServ.GetPage(`${this.baseURL}BranchPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}&name=${this.searchTerm}`).subscribe({
      next:(value:any)=>{
        

        this.branches=value.list;
        this.totalBranches=value.totalCount;
        this.pagesNum=value.totalPages;

        this.startIndex=(this.currentPage-1)*this.selectedEntries
        this.endIndex= this.branches.length + this.startIndex

        


      },
      error:(error)=> {
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
    })
  }





  StatusName(s:Status){
    return Status[s]
  }

  getCityName(id:number){
    return this.cities.find(c=>c.id==id)?.name;
  }

  deleteBranch(id:number){
    this.dbSub = this.branchServ.Delete(this.baseURL+"Branches?id="+id).subscribe({
      next:()=>{
        this.branches=this.branches.filter(b=>b.id!= id);
        console.log(this.branches);

      },
      error:(error)=>{
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
    })
  }

  onEntriesChange(){
    this.getBranches();
  }

  onSearchChange(){
    this.getBranches();

  }

  previousPage(){
    if(this.currentPage!=1)
    {
      this.currentPage--;
      this.getBranches()
    }

  }
  goToPage(pageIndex:number)
  {
    this.currentPage=pageIndex
    this.getBranches()
  }
  nextPage(){
    if(this.currentPage!=this.pagesNum)
      {
        this.currentPage++;
        this.getBranches()
      }
  }

}
