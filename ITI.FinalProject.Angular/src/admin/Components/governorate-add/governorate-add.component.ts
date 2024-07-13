import { Status } from './../../Enums/Status';
import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { GenericService } from '../../Services/generic.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-governorate-add',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './governorate-add.component.html',
  styleUrl: './governorate-add.component.css'
})
export class GovernorateAddComponent implements OnDestroy {

  baseURL:string="http://localhost:5241/api/";

  govStatus:any;

  options:string[];

  form:FormGroup;

  gSub:any;

  constructor(
    private governorateService:GenericService<IGovernorate, IGovernorateInsert, IGovernorateUpdate>,
    private router:Router
  ) {

    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(4)]),
      status: new FormControl(-1, [Validators.required, Validators.min(0)])
    })

    this.govStatus = Status;

    this.options = Object.keys(this.govStatus).map(key => this.govStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" )
  }

  ngOnDestroy(): void {
    if (this.gSub) {
      this.gSub.unsubscribe();
    }
  }

  get getName(){
    return this.form.controls['name'];
  }

  get getStatus(){
    return this.form.controls['status'];
  }

  addGovernorate() {

    if (this.form.status == 'INVALID') {
      
      
      
      
      
      alert("Please Input All the Required Fields with Valid Values");
      return;
    }
    console.log(this.form.value);
    let governorate:IGovernorateInsert = {
      name:this.getName.value,
      status:Number(this.getStatus.value)
    }
    this.gSub = this.governorateService.Add(this.baseURL+"Governorate", governorate).subscribe({
      next: data => {
        this.router.navigate(["/admin/governorate"]);
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
}
