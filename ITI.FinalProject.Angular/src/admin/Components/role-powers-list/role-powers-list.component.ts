import { Component, OnDestroy, OnInit } from '@angular/core';
import { IRolePower } from '../../DTOs/DisplayDTOs/IRolePower';
import { IRolePowerInsert } from '../../DTOs/InsertDTOs/IRolePowerInsert';
import { IRolePowerUpdate } from '../../DTOs/UpdateDTOs/IRolePowerUpdate';
import { GenericService } from '../../Services/generic.service';
import { Status } from '../../Enums/Status';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { error } from 'console';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-role-powers-list',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './role-powers-list.component.html',
  styleUrl: './role-powers-list.component.css'
})
export class RolePowersListComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/";
  data: IRolePower[] = [];
  selectedEntries = 8;
  searchTerm = '';
  currentPage = 1;
  totalPages = 0;
  totalCount = 0;
  
  
  
  

  rSub:any;

  showModal:boolean;

  roleName:string;

  aSub:any;

  remSub:any;

  isValid:boolean;

  constructor(
    private roleService: GenericService<IRolePower, IRolePowerInsert, IRolePowerUpdate>,
    private router:Router
  ) {
    this.showModal = false;
    this.roleName = ''
    this.isValid = true;
  }

  ngOnInit(): void {
    this.loadRolePowers();
  }

  ngOnDestroy(): void {
    if (this.rSub) {
      this.rSub.unsubscribe();
    }
    if (this.aSub) {
      this.aSub.unsubscribe();
    }
    if (this.remSub) {
      this.remSub.unsubscribe();
    }
  }

  loadRolePowers(): void {
    

    this.rSub = this.roleService.GetPage(`${this.baseURL}RolePowerPage?pageNumber=${this.currentPage}&pageSize=${this.selectedEntries}&name=${this.searchTerm}`).subscribe({
      next: (data:any) =>{
        console.log("data",data);

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
    
    this.loadRolePowers();
  }

  onSearchChange(): void {
    this.currentPage = 1;
    
    this.loadRolePowers();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      
      this.loadRolePowers();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      
      this.loadRolePowers();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      
      this.loadRolePowers();
    }
  }

  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  

  
  
  
  
  
  
  
  
  
  
  

  getStatusText(status: Status): string {
    return Status[status];
  }

  
  
  

  toggleModal() {
    this.showModal = !this.showModal;
    this.roleName = '';
    this.isValid = true;
  }

  closeModal() {
    this.showModal = false;
    this.roleName = '';
    this.isValid = true;
  }

  stopPropagation(event: MouseEvent) {
    event.stopPropagation();
  }

  addRole(){
    if (this.roleName.length < 3) {
      alert("Role Name Is Required and must be more than 2 characters");
      return;
    }
    let role:IRolePowerInsert = {
      roleName:this.roleName
    }
    this.aSub = this.roleService.Add(this.baseURL+"RolePowers", role).subscribe({
      next: data => {
        this.toggleModal()
        
        
        const currentUrl = this.router.url;
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
          this.router.navigate([currentUrl]);
        });
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

  roleNameModified() {
    if (this.roleName.length < 3) {
      this.isValid = false;
    }else{
      this.isValid = true;
    }
  }

  deleteRole(id:string){
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!"
    }).then((result) => {
      if (result.isConfirmed) {

        this.remSub = this.roleService.Delete(`${this.baseURL}RolePowers/${id}`).subscribe({
          next: data => {
            console.log(data);
            const currentUrl = this.router.url;
            this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
              this.router.navigate([currentUrl]);
            });
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
    });
  }
}
