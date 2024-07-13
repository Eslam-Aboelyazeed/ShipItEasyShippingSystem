import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { IBranch } from '../../DTOs/DisplayDTOs/IBranch';
import { IBranchInsert } from '../../DTOs/InsertDTOs/IBranchInsert';
import { IBranchUpdate } from '../../DTOs/UpdateDTOs/IBranchUpdate';
import { GenericService } from '../../Services/generic.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { Status } from '../../Enums/Status';
import { IRepresentative } from '../../DTOs/DisplayDTOs/IRepresentative';
import { IRepresentativeInsert } from '../../DTOs/InsertDTOs/IRepresentativeInsert';
import { IRepresentativeUpdate } from '../../DTOs/UpdateDTOs/IRepresentativeUpdate';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-representative-list',
  standalone: true,
  imports: [CommonModule,RouterLink,FormsModule],
  templateUrl: './representative-list.component.html',
  styleUrl: './representative-list.component.css'
})
export class RepresentativeListComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/"
  branches:IBranch[]=[]
  governorates:IGovernorate[]= []
  representatives:IRepresentative[]=[]
  selectedEntries:number=5
  searchTerm:string=""
  currentPage:number=1
  totalRepresentatives:number=0
  pagesNum:number=1
  startIndex:number=1
  endIndex:number=1
  

  bSub:any;
  gSub:any;
  rSub:any;
  drSub:any;


  constructor(
    public branchServ:GenericService<IBranch,IBranchInsert,IBranchUpdate>,
    public governorateServ:GenericService<IGovernorate,IBranchInsert,IBranchUpdate>,
    public representativeServ:GenericService<IRepresentative,IRepresentativeInsert,IRepresentativeUpdate>

  ) {
  }

  ngOnDestroy(): void {
    if (this.bSub != undefined) {
      this.bSub.unsubscribe();
    }
    if (this.gSub != undefined) {
      this.gSub.unsubscribe();
    }
    if (this.rSub != undefined) {
      this.rSub.unsubscribe();
    }
    if (this.drSub != undefined) {
      this.drSub.unsubscribe();
    }
  }

  ngOnInit(): void {

    
    this.bSub = this.branchServ.GetAll(this.baseURL+"branches").subscribe({
      next:(value)=> {
        this.branches=value;
      },
      error: error =>{
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

    
    this.gSub = this.governorateServ.GetAll(this.baseURL+"governorate").subscribe({
      next:(value)=> {
        this.governorates=value
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

    
    this.getRepresentatives()
  }

  getRepresentatives(){
    this.rSub = this.representativeServ.GetPage(`${this.baseURL}RepresentativePage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}&name=${this.searchTerm}`).subscribe({
      next:(value:any)=>{
        console.log(value);

        this.representatives=value.list;
        console.log(this.representatives);

        this.totalRepresentatives=value.totalCount;
        this.pagesNum=value.totalPages;

        this.startIndex=(this.currentPage-1)*this.selectedEntries
        this.endIndex= this.representatives?.length + this.startIndex

        


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


  deleteRepresentative(id:string){
    this.drSub = this.representativeServ.Delete(this.baseURL+"representative/"+id).subscribe({
      next:(value)=>{
        console.log(value);
        this.representatives=this.representatives.filter(r=>r.id!=id);

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

  getBranchName(id:number)
  {
    return this.branches.find(b=> b.id==id)?.name
  }
  getGovNames(govIds:number[]){
    let govNames:any[]=[]
    govIds.forEach(id => {
      govNames.push(this.governorates.find(g=>g.id==id)?.name)
    });
    return govNames;

  }

  onEntriesChange(){
    this.getRepresentatives();
  }

  onSearchChange(){
    this.getRepresentatives();

  }

  previousPage(){
    if(this.currentPage!=1)
    {
      this.currentPage--;
      this.getRepresentatives()
    }

  }
  goToPage(pageIndex:number)
  {
    this.currentPage=pageIndex
    this.getRepresentatives()
  }
  nextPage(){
    if(this.currentPage!=this.pagesNum)
      {
        this.currentPage++;
        this.getRepresentatives()
      }
  }
}
