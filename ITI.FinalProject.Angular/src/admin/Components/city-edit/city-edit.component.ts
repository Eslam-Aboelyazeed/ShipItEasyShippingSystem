import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CityService } from '../../Services/city.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Status } from '../../Enums/Status';
import { IDisplayCity } from '../../DTOs/DisplayDTOs/IDisplayCity';
import { IUpdateCity } from '../../DTOs/UpdateDTOs/IUpdateCity';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { GenericService } from '../../Services/generic.service';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import { IOption } from '../../../app/shared-module/DTOs/DisplayDTOs/IOption';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-city-edit',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './city-edit.component.html',
  styleUrl: './city-edit.component.css'
})
export class CityEditComponent implements OnInit, OnDestroy{
  baseURL:string="http://localhost:5241/api/";
  governorates:IOption[]=[];
  cityForm:FormGroup;
  cityId:number= 0;
  cityStatus:any;
  options:string[];
  
  opSub:any;
  cSub:any;
  acSub:any;
  constructor(
  private cityService:CityService,
  private route:ActivatedRoute,
  private formBuilder:FormBuilder,
  private router:Router,
  private governorateService:GenericService<IGovernorate,IGovernorateInsert,IGovernorateUpdate>
  ){
    this.cityForm=this.formBuilder.group({
      id:0,
      name:'',
      status:-1,
      normalShippingCost:0,
      pickupShippingCost:0,
      governorateId:0
    });
    
    
    
    
    
    this.cityStatus = Status;

    this.options = Object.keys(this.cityStatus).map(key => this.cityStatus[key]);

    this.options = this.options.filter(el => typeof(el) != "number" )
  }

  ngOnInit():void{

    this.cityId = Number(this.route.snapshot.paramMap.get('id'));
    console.log(this.cityId);
     
     
     
    if(this.cityId !=0){
      this.cSub = this.cityService.GetById(`${this.baseURL}Cities/${this.cityId}`).subscribe({
        next:(city:IDisplayCity|undefined)=>{
      if(this.cityForm&&city){
        city.id=this.cityId;
        console.log(city);
        this.cityForm.patchValue(city);
        this.opSub = this.governorateService.GetOptions(this.baseURL+"governorateOptions").subscribe({
          next:(data) => {
            this.governorates = data;
            
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
        });
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
    }
  }

  ngOnDestroy(): void {
    if (this.opSub) {
      this.opSub.unsubscribe();
    }
    if (this.cSub != undefined) {
      this.cSub.unsubscribe();
    }
    if (this.acSub != undefined) {
      this.acSub.unsubscribe();
    }
  }

  updateCity():void{
    
    if (this.cityForm.valid) {
      const cityUpdate: IUpdateCity = {...this.cityForm.value, status: Number(this.cityForm.controls['status'].value)};
      console.log(cityUpdate);
      this.acSub = this.cityService.Edit(`${this.baseURL}Cities/${this.cityId}`, cityUpdate).subscribe({
        next: (res) => {
          console.log(res);
          this.router.navigate(['/admin/city']);
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
        }
      });
    }
  }
}
