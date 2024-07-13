import { IEmployeeUpdate } from './../../DTOs/UpdateDTOs/IEmployeeUpdate';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, FormBuilder ,FormsModule,
  ReactiveFormsModule,} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeService } from './../../Services/employee.service';
import { IEmployee } from '../../DTOs/DisplayDTOs/IEmployee';
import { Status } from '../../Enums/Status';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-employee-edit',
  standalone: true,
  imports:  [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './employee-edit.component.html',
  styleUrls: ['./employee-edit.component.css']
})
export class EmployeeEditComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/";
  employeeForm: FormGroup;
  statusOptions = Object.keys(Status);
  successAlert = false;
  deleteAlert = false;

  employeeId:string;

  govStatus:any;

  options:string[];

  eeSub:any;
  eSub:any;

  constructor(
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.employeeForm = this.formBuilder.group({
      id: [0],    
      status: [Status.Inactive],
    });
    this.employeeId = '';

    this.govStatus = Status;

    this.options = Object.keys(this.govStatus).map(key => this.govStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" )
  }

  ngOnDestroy(): void {
    if (this.eeSub != undefined) {
      this.eeSub.unsubscribe();
    }
    if (this.eSub != undefined) {
      this.eSub.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id') || '';
    if (this.employeeId) {
      this.eSub = this.employeeService
        .GetById(`${this.baseURL}Employees/${this.employeeId}`)
        .subscribe((employee: IEmployee | undefined) => {
          if (this.employeeForm && employee) {
            console.log(employee);
            
            this.employeeForm.controls['status'].setValue(employee.status);
          }
        });
    }
  }
///
  onSubmit() {
    if (this.employeeForm.valid) {
      const updatedEmployee: IEmployeeUpdate = {
        id: this.employeeId,
        status: Number(this.employeeForm.controls['status'].value)
      };
      console.log(updatedEmployee);
      
      this.eeSub = this.employeeService.Edit(`${this.baseURL}Employees/${this.employeeId}`,updatedEmployee).subscribe({
        next:() => {
          
          
          this.router.navigate(['/admin/employee'])
        },
       error: (error: any) => {
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
  }
}
