import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, MinValidator, Validators,ReactiveFormsModule } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { GenericService } from '../../Services/generic.service';
import { IRepresentativeInsert } from '../../DTOs/InsertDTOs/IRepresentativeInsert';
import { IRepresentative } from '../../DTOs/DisplayDTOs/IRepresentative';
import { IRepresentativeUpdate } from '../../DTOs/UpdateDTOs/IRepresentativeUpdate';
import { IBranch } from '../../DTOs/DisplayDTOs/IBranch';
import { IBranchInsert } from '../../DTOs/InsertDTOs/IBranchInsert';
import { IBranchUpdate } from '../../DTOs/UpdateDTOs/IBranchUpdate';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { routes } from '../../../app/app.routes';
import { IOption } from '../../../app/shared-module/DTOs/DisplayDTOs/IOption';
import Swal from 'sweetalert2';
import { error } from 'console';

@Component({
  selector: 'app-representative-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule,RouterLink],
  templateUrl: './representative-form.component.html',
  styleUrls:[
    './representative-form.component.css'
  ]
})
export class RepresentativeFormComponent implements OnInit, OnDestroy {

  constructor(
    public route:ActivatedRoute,
    public representativeServ:GenericService<IRepresentative,IRepresentativeInsert,IRepresentativeUpdate>,
    public branchServ:GenericService<IBranch,IBranchInsert,IBranchUpdate>,
    public governorateServ:GenericService<IGovernorate,IBranchInsert,IBranchUpdate>,
    public routing:Router
  ) {

  }
  
  ngOnDestroy(): void {
    if (this.arSub != undefined) {
      this.arSub.unsubscribe();
    }
    if (this.rSub != undefined) {
      this.rSub.unsubscribe();
    }
    if (this.bSub != undefined) {
      this.bSub.unsubscribe();
    }
    if (this.gSub != undefined) {
      this.gSub.unsubscribe();
    }
    if (this.erSub != undefined) {
      this.erSub.unsubscribe();
    }
    if (this.irSub != undefined) {
      this.irSub.unsubscribe();
    }
  }

  baseURL:string="http://localhost:5241/api/";

  representative:any;
  representativeId:string=""
  branches:IOption[]=[]
  governorates:IOption[]= []
  selectedGovernorate:number[]=[]
  govLen:number =this.selectedGovernorate.length
  govFlag:boolean=false;

  arSub:any;
  rSub:any;
  bSub:any;
  gSub:any;
  erSub:any;
  irSub:any;



 representativeForm = new FormGroup({
    
    userFullName: new FormControl('', [Validators.required,Validators.minLength(2)]),
    email: new FormControl('', [Validators.required,Validators.email]),
    password: new FormControl('', [Validators.pattern("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$")]),
    oldPassword: new FormControl('', [Validators.pattern("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$")]),
    newPassword: new FormControl('', [Validators.pattern("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$")]),
    userBranchId: new FormControl(0, [Validators.required, Validators.min(1)]),
    governorateIds: new FormControl(this.selectedGovernorate, [Validators.minLength(1)]),
    userPhoneNo: new FormControl('', [Validators.required,Validators.maxLength(11),Validators.minLength(11)]),
    userAddress: new FormControl('', [Validators.required]),
    discountType: new FormControl(0, [Validators.required]),
    companyPercentage: new FormControl('', [Validators.required]),
    userStatus: new FormControl(0,[Validators.required]),


  })

  get getName()
  {
    return this.representativeForm.controls['userFullName'];
  }
  get getEmail()
  {
    return this.representativeForm.controls['email'];
  }
  get getPassword()
  {
    return this.representativeForm.controls['password'];
  }
  get getOldPassword()
  {
    return this.representativeForm.controls['oldPassword'];
  }
  get getNewPassword()
  {
    return this.representativeForm.controls['newPassword'];
  }
  get getBranch()
  {
    return this.representativeForm.controls['userBranchId'];
  }
  get getGovernorate()
  {
    return this.representativeForm.controls['governorateIds'];
  }
  get getPhone()
  {
    return this.representativeForm.controls['userPhoneNo'];
  }
  get getAddress()
  {
    return this.representativeForm.controls['userAddress'];
  }
  get getDiscount()
  {
    return this.representativeForm.controls['discountType'];
  }
  get getCompanyPercentage()
  {
    return this.representativeForm.controls['companyPercentage'];
  }

  ngOnInit(): void {
    
    this.arSub = this.route.params.subscribe({
      next:(params)=> {
        this.representativeId=params['id'];

        if (this.representativeId) {
          this.rSub = this.representativeServ.GetById(this.baseURL+"representative/"+this.representativeId).subscribe({
            next: (value) => {
              this.representative = value;
              console.log(this.representative);
              
              this.getName.setValue(this.representative?.userFullName)
              this.getEmail.setValue(this.representative.email)
              this.getBranch.setValue(this.representative.userBranchId)
              this.getGovernorate.setValue(this.representative.governorateIds)
              this.getPhone.setValue(this.representative.userPhoneNo)
              this.getAddress.setValue(this.representative.userAddress)
              this.getDiscount.setValue(this.representative.discountType)
              this.getCompanyPercentage.setValue(this.representative.companyPercentage)


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
              };

            },
          })
        }

      },
      error: error => {
        Swal.fire({
          icon: "error",
          title: "Error",
          text: "Something went wrong, please try again later",
        })
      }
    })

    
    this.bSub = this.branchServ.GetOptions(this.baseURL+"branchOptions").subscribe({
      next:(value)=> {
        
        this.branches = value;
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

    
    this.gSub = this.governorateServ.GetOptions(this.baseURL+"governorateOptions").subscribe({
      next:(value)=> {
        
        this.governorates = value;
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


  onSubmit(){

    this.representativeForm.controls['governorateIds'].setValue(this.selectedGovernorate);
    this.representativeForm.controls['userBranchId'].setValue(Number(this.getBranch.value));
    this.representativeForm.controls['discountType'].setValue(Number(this.getDiscount.value));
    let Rep:IRepresentativeUpdate = {
      id:this.representativeId,
      userFullName:this.getName.value || "",
      userStatus: Number(this.representativeForm.controls['userStatus'].value),
      companyPercentage: Number(this.getCompanyPercentage.value),
      discountType:Number(this.getDiscount.value),
      email:this.getEmail.value || "",
      governorateIds:this.getGovernorate.value || [],
      userAddress:this.getAddress.value || "",
      userBranchId: Number(this.getBranch.value),
      userPhoneNo:this.getPhone.value || "",
      newPassword:this.getNewPassword.value || "",
      oldPassword: this.getOldPassword.value || ""
    }
    console.log(Rep);


    if(this.representativeId){
      console.log("Rep",Rep);
      
      this.erSub = this.representativeServ.Edit(this.baseURL+"representative/"+this.representativeId,Rep).subscribe({
        next:(value)=> {
          console.log(value);
          this.routing.navigate(['admin/representative']);

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
    }else{
      
      let newRep:IRepresentativeInsert={
        companyPercentage: Number(this.getCompanyPercentage.value),
        discountType: Number(this.getDiscount.value),
        email: this.getEmail.value!,
        governorateIds:this.representativeForm.controls['governorateIds'].value!,
        password:this.getPassword.value!,
        userAddress:this.getAddress.value!,
        userBranchId:Number(this.getBranch.value),
        userFullName:this.getName.value!,
        userPhoneNo:this.getPhone.value!,
        userStatus:Number(this.representativeForm.controls['userStatus'].value)
      } 
      console.log(newRep);
      this.irSub = this.representativeServ.Add(this.baseURL+"representative",newRep).subscribe({
        next:(value)=> {
          console.log(value);
          this.routing.navigate(['admin/representative']);

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


  }


  selectGov(e:any){

    if(e.target['checked']){
      this.selectedGovernorate.push(Number(e.target.value))
      this.govLen++;

    }
    else{
      this.selectedGovernorate=this.selectedGovernorate.filter(n=> n !== Number(e.target.value));
      console.log(this.selectedGovernorate);

      this.govLen--;
    }

  }


}
