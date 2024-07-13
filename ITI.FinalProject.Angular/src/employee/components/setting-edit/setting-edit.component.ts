import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { GenericService } from '../../../admin/Services/generic.service';
import { ISettings } from '../../DTOs/DisplayDTOs/ISettings';
import { IAddSettings } from '../../DTOs/InsertDTOs/IAddSettings';
import { IUpdateSettings } from '../../DTOs/UpdateDTOs/IUpdateSettings';
import { ActivatedRoute, Router } from '@angular/router';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-setting-edit',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule,FormsModule],
  templateUrl: './setting-edit.component.html',
  styleUrl: './setting-edit.component.css'
})
export class SettingEditComponent implements OnInit, OnDestroy{

  baseUrl:string="http://localhost:5241/api/";

  settingEdit: FormGroup;
  settingId: number=0;

  sSub:any;
  esSub:any;

  constructor(
    private formBuilder: FormBuilder,
    private settingService:GenericService<ISettings,IAddSettings,IUpdateSettings>,
    private router:Router,
    private route:ActivatedRoute
  ) {
    this.settingEdit = this.formBuilder.group({
      id: 0,
      baseWeight:0,
      additionalFeePerKg:0,
      villageDeliveryFee:0,
      ordinaryShippingCost:0,
      twentyFourHoursShippingCost:0,
      fifteenDayShippingCost:0
      });
   }

  ngOnDestroy(): void {
    throw new Error('Method not implemented.');
  }
  ngOnInit(): void {
    this.settingId=Number(this.route.snapshot.paramMap.get('id'));
    console.log(this.settingId);
    if(this.settingId !=0){
      this.sSub = this.settingService.GetById(`${this.baseUrl}Settings/${this.settingId}`).subscribe({
      next:  (setting:ISettings|undefined)=>{
      if(this.settingEdit&&setting){
        setting.id=this.settingId;
        console.log(setting);
        this.settingEdit.patchValue(setting);
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
  updateSettings():void{
    console.log(this.settingEdit.value);
    if (this.settingEdit.valid) {
      const settingUpdate: IUpdateSettings = this.settingEdit.value;
      console.log(settingUpdate);
      this.esSub = this.settingService.Edit(`${this.baseUrl}Settings/${this.settingId}`, settingUpdate).subscribe({
        next: (res) => {
          console.log(res);
          this.router.navigate(['/employee/setting']);
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
