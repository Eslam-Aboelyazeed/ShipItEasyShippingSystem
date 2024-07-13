import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { GenericService } from '../../Services/generic.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Status } from '../../Enums/Status';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-governorate-edit',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './governorate-edit.component.html',
  styleUrl: './governorate-edit.component.css'
})
export class GovernorateEditComponent implements OnInit, OnDestroy {

  baseURL:string="http://localhost:5241/api/";

  govStatus:any;

  options:string[];

  form:FormGroup;

  gSub:any;

  govSub:any;

  arSub:any;

  constructor(
    private governorateService:GenericService<IGovernorate, IGovernorateInsert, IGovernorateUpdate>,
    private router:Router,
    private activatedRoute:ActivatedRoute
  ) {

    this.form = new FormGroup({
      id: new FormControl(0, Validators.required),
      name: new FormControl('', [Validators.required, Validators.minLength(4)]),
      status: new FormControl(-1, [Validators.required, Validators.min(0)])
    })

    this.govStatus = Status;

    this.options = Object.keys(this.govStatus).map(key => this.govStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" )
  }

   ngOnInit(): void {
    this.arSub = this.activatedRoute.params.subscribe({
      next: id => {
        console.log(id['id']);
        this.govSub = this.governorateService.GetById(`${this.baseURL}Governorate/${id['id']}`).subscribe({
          next: data => {
            if (data) {
              this.form.patchValue(data);
            }
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
      error: error => {
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "Something went wrong, please try again later",
        })
      }
    })

  }

  ngOnDestroy(): void {
    if (this.gSub) {
      this.gSub.unsubscribe();
    }

    if (this.govSub) {
      this.govSub.unsubscribe();
    }

    if (this.arSub) {
      this.arSub.unsubscribe();
    }
  }

  get getName(){
    return this.form.controls['name'];
  }

  get getStatus(){
    return this.form.controls['status'];
  }

  editGovernorate() {

    if (this.form.status == 'INVALID') {
      
      
      
      
      
      alert("Please Input All the Required Fields with Valid Values");
      return;
    }
    console.log(this.form.value);
    let governorate:IGovernorateUpdate = {
      id:Number(this.form.controls['id'].value),
      name:this.getName.value,
      status:Number(this.getStatus.value)
    }
    this.gSub = this.governorateService.Edit(`${this.baseURL}Governorate/${this.form.controls['id'].value}`, governorate).subscribe({
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
