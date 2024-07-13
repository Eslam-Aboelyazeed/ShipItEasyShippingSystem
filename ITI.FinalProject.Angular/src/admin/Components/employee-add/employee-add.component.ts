import { IBranch } from '../../DTOs/DisplayDTOs/IBranch';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EmployeeService } from '../../Services/employee.service';
import { CommonModule } from '@angular/common';
import { BranchService } from '../../Services/branch.service';
import { Status } from '../../Enums/Status';
import { GenericService } from '../../Services/generic.service';
import { IRolePower } from '../../DTOs/DisplayDTOs/IRolePower';
import { IRolePowerInsert } from '../../DTOs/InsertDTOs/IRolePowerInsert';
import { IRolePowerUpdate } from '../../DTOs/UpdateDTOs/IRolePowerUpdate';
import { IEmployeeInsert } from '../../DTOs/InsertDTOs/IEmployeeInsert';
import { IOption } from '../../../app/shared-module/DTOs/DisplayDTOs/IOption';
import Swal from 'sweetalert2';



@Component({
  selector: 'app-employee-add',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './employee-add.component.html',
  styleUrls: ['./employee-add.component.css'],
})
export class EmployeeAddComponent implements OnInit, OnDestroy {
  baseURL:string="http://localhost:5241/api/";
  addEmployeeForm: FormGroup;
  branches: IOption[] = [];
  roles:string[]=[]

  rSub:any;
  bSub:any;
  eSub:any;

  govStatus:any;

  options:string[];

  constructor(
    private formBuilder: FormBuilder,
    private employeeService: EmployeeService,
    private branchService: BranchService,
    private roleService:GenericService<IRolePower,IRolePowerInsert,IRolePowerUpdate>,
    private router: Router
  ) {
    this.addEmployeeForm = this.formBuilder.group({
      fullName: ['', Validators.required],
      userName: ['', Validators.required],
      passwordHash: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      address: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      branchId: [0, [Validators.required, Validators.min(1)]],
      role: ['', Validators.required],
      status: [-1, [Validators.required, Validators.min(0)]],
    });

    this.govStatus = Status;

    this.options = Object.keys(this.govStatus).map(key => this.govStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" )
  }
  ngOnDestroy(): void {
    if (this.rSub != undefined) {
      this.rSub.unsubscribe();
    }
    if (this.bSub != undefined) {
      this.bSub.unsubscribe();
    }
    if (this.eSub != undefined) {
      this.eSub.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.loadBranches();
    this.getRoles()
  }

  getRoles(){

    this.rSub = this.roleService.GetOptions(`${this.baseURL}roleOptions`).subscribe({
      next:(value) =>{
        value.forEach((element) =>{
          this.roles.push(element.name)
        });
        console.log(this.roles);

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



  loadBranches(){
    const url = this.baseURL+'branchOptions';
    this.bSub = this.branchService.GetOptions(url).subscribe({next : (branches) =>{

      
      this.branches=branches;
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

  onSubmit() {
    if (this.addEmployeeForm.valid) {
      let employee:IEmployeeInsert = {
        address: this.addEmployeeForm.controls['address'].value,
        branchId: Number(this.addEmployeeForm.controls['branchId'].value),
        email: this.addEmployeeForm.controls['email'].value,
        fullName: this.addEmployeeForm.controls['fullName'].value,
        passwordHash:this.addEmployeeForm.controls['passwordHash'].value,
        phoneNumber:this.addEmployeeForm.controls['phoneNumber'].value,
        role:this.addEmployeeForm.controls['role'].value,
        status:Number(this.addEmployeeForm.controls['status'].value),
        userName:this.addEmployeeForm.controls['userName'].value
      }
      console.log("emp",employee);
      this.eSub = this.employeeService.Add(this.baseURL+'Employees', employee).subscribe(
        {next:() => {
          alert('Employee added successfully');
          this.router.navigate(["/admin/employee"])
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
        }}
      );
    } else {
      Swal.fire({
        icon: "error",
        title: "Error",
        text: "Please fill the inputs with valid data",
      })
    }
  }
}
