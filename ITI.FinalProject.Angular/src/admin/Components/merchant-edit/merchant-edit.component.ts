import { ICity } from '../../DTOs/DisplayDTOs/ICity';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MerchantService } from '../../Services/merchant.service';
import { GovernorateService } from '../../Services/governorate.service';
import { CityService } from '../../Services/city.service';
import { BranchService } from '../../Services/branch.service';
import { IDisplayMerchant } from '../../DTOs/DisplayDTOs/IDisplayMerchant';
import {
  FormBuilder,
  FormGroup,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { IDisplaySpecialPackage } from '../../DTOs/DisplayDTOs/IDisplaySpecialPackage';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IUpdateSpecialPackage } from '../../DTOs/UpdateDTOs/IUpdateSpecialPackage';
import { Status } from '../../Enums/Status';
import { IBranch } from '../../DTOs/DisplayDTOs/IBranch';
import { IUpdateMerchant } from '../../DTOs/UpdateDTOs/IUpdateMerchant';
import { Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-merchant-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './merchant-edit.component.html',
  styleUrls: ['./merchant-edit.component.css'],
})
export class MerchantEditComponent implements OnInit, OnDestroy {
  
  baseURL:string="http://localhost:5241/api/";
  merchantForm: FormGroup;
  statusOptions = Object.keys(Status);
  successAlert = false;
  deleteAlert = false;

  merchantId:string;

  govStatus:any;

  options:string[];

  eeSub:any;
  eSub:any;

  constructor(
    private merchantService: MerchantService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.merchantForm = this.formBuilder.group({
      id: [0],
      
      
      
      
      
      
      
      
      status: [Status.Inactive],
    });
    this.merchantId = '';

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
    this.merchantId = this.route.snapshot.paramMap.get('id') || '';
    if (this.merchantId) {
      this.eSub = this.merchantService
        .GetById(`${this.baseURL}merchant/${this.merchantId}`)
        .subscribe((merchant: IDisplayMerchant | undefined) => {
          if (this.merchantForm && merchant) {
            console.log(merchant);
            
            this.merchantForm.controls['status'].setValue(merchant.status);
          }
        });
    }
  }
///
  onSubmit() {
    if (this.merchantForm.valid) {
      const updatedmerchant: IUpdateMerchant = {
        id: this.merchantId,
        status: Number(this.merchantForm.controls['status'].value)
      };
      console.log(updatedmerchant);
      
      this.eeSub = this.merchantService.Edit(`${this.baseURL}merchant/${this.merchantId}`,updatedmerchant).subscribe({
        next:() => {
          
          
          this.router.navigate(['/admin/merchant'])
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
