import { Component, OnDestroy, OnInit} from '@angular/core';
import { CityService } from '../../Services/city.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Status } from '../../Enums/Status';
import { IAddCity } from '../../DTOs/InsertDTOs/IAddCity';
import { CommonModule } from '@angular/common';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { GenericService } from '../../Services/generic.service';
import { IGovernorateInsert } from '../../DTOs/InsertDTOs/IGovernorateInsert';
import { IGovernorateUpdate } from '../../DTOs/UpdateDTOs/IGovernorateUpdate';
import Swal from 'sweetalert2';
import { IOption } from '../../../app/shared-module/DTOs/DisplayDTOs/IOption';

@Component({
  selector: 'app-city-add',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './city-add.component.html',
  styleUrl: './city-add.component.css'
})
export class CityAddComponent implements OnInit, OnDestroy{
  baseURL:string="http://localhost:5241/api/";
  governorates:IOption[]=[];
  cityForm:FormGroup;
  cityStatus:any;
  
  opSub:any;
  cSub:any;

  options:string[];
  constructor(
    private cityService:CityService,
    private route:ActivatedRoute,
    private formBuilder:FormBuilder,
    private router:Router,
    private governorateService:GenericService<IGovernorate,IGovernorateInsert,IGovernorateUpdate>
  ){
      this.cityForm=this.formBuilder.group({
        name:'',
        status:-1,
        normalShippingCost:0,
        pickupShippingCost:0,
        governorateId:0,
      });

      this.cityStatus = Status;

      this.options = Object.keys(this.cityStatus).map(key => this.cityStatus[key]);
  
      this.options = this.options.filter(el => typeof(el) != "number" )
    }

  ngOnInit(): void {
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

  ngOnDestroy(): void {
    if (this.opSub) {
      this.opSub.unsubscribe();
    }
    if (this.cSub != undefined) {
      this.cSub.unsubscribe();
    }
  }


    addCity(){
      const city:IAddCity = {...this.cityForm.value, status: Number(this.cityForm.controls['status'].value)};
      console.log(city);
      this.cSub = this.cityService.Add(this.baseURL+"Cities",city).subscribe({next: (res)=>{
        console.log(res);
        this.router.navigate(['/admin/city/']);

      },
      error: (error)=>{
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
