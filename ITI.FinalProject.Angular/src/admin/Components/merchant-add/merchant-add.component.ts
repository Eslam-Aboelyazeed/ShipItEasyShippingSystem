

































































































































































import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';
import { MerchantService } from '../../Services/merchant.service';
import { CityService } from '../../Services/city.service';
import { BranchService } from '../../Services/branch.service';
import { GovernorateService } from '../../Services/governorate.service';
import { IGovernorate } from '../../DTOs/DisplayDTOs/IGovernorate';
import { IDisplaySpecialPackage } from '../../DTOs/DisplayDTOs/IDisplaySpecialPackage';
import { CommonModule } from '@angular/common';
import { IAddSpecialPackage } from '../../DTOs/InsertDTOs/IAddSpecialPackage';
import { IDisplayCity } from '../../../merchant/DTOs/Display DTOs/IDisplayCity';
import { IDisplayBranch } from '../../../merchant/DTOs/Display DTOs/IDisplayBranch';
import { Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-merchant-add',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './merchant-add.component.html',
  styleUrl: './merchant-add.component.css',
})
export class MerchantAddComponent implements OnInit {
  addMerchantForm: FormGroup;
  newPackageForm: FormGroup;
  governorates: any[] = [];
  cities: any[] = [];
  filteredCities: any[] = [];
  branches: any[] = [];
  filteredBranches: any[] = [];
  specialPackages: IAddSpecialPackage[] = [];
  displaySpecialPackage: IDisplaySpecialPackage[] = [];
  addSpecialPackage = false;
  subscriptions: Subscription = new Subscription();

  constructor(
    private formBuilder: FormBuilder,
    private merchantService: MerchantService,
    private governorateService: GovernorateService,
    private cityService: CityService,
    private branchService: BranchService,
    private router: Router
  ) {
    this.addMerchantForm = this.formBuilder.group({
      storeName: [''],
      userName: ['', Validators.required],
      passwordHash: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      address: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      merchantPayingPercentageForRejectedOrders: [
        0,
        [Validators.required, Validators.min(1)],
      ],
      specialPickupShippingCost: [0],
      status: [1, Validators.required],
      cityId: [0, [Validators.required, , Validators.min(1)]],
      governorateId: [0, [Validators.required, Validators.min(1)]],
      branchId: [0, [Validators.required, Validators.min(1)]],
      specialPackages: [[]],
    });

    this.newPackageForm = this.formBuilder.group({
      governorateId: [0, [Validators.required, Validators.min(1)]],
      cityId: [0, [Validators.required, Validators.min(1)]],
      shippingPrice: [0, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadDropdownData();
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  loadDropdownData() {
    const url = 'http://localhost:5241/api/';
    const citySub = this.cityService
      .GetOptions(url + 'cityOptions')
      .subscribe({
        next:(data) => {
        
        this.cities = data;
        
        this.filteredCities = data;
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

    const governorateSub = this.governorateService
      .GetOptions(url + 'governorateOptions')
      .subscribe({
        next:(data) => {
        
        
        
        this.governorates = data;
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

    const branchSub = this.branchService
      .GetOptions(url + 'branchOptions')
      .subscribe({
        next:(data) => {
        
        this.branches = data;
        
        this.filteredBranches = data;
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

    this.subscriptions.add(citySub);
    this.subscriptions.add(governorateSub);
    this.subscriptions.add(branchSub);
  }

  
  filterCities(governorateId: number) {
    this.filteredCities = this.cities.filter(
      (city) => city.dependentId == governorateId
    );
    this.addMerchantForm.get('cityId')?.setValue('');
    this.filteredBranches = [];
    this.addMerchantForm.get('branchId')?.setValue('');
  }

  
  filterBranches(cityId: number) {
    this.filteredBranches = this.branches.filter(
      (branch) => branch.dependentId == cityId
    );
    this.addMerchantForm.get('branchId')?.setValue('');
  }

  
  toggleAddNewPackage() {
    this.addSpecialPackage = !this.addSpecialPackage;
  }

  
  saveSpecialPackage() {
    if (this.newPackageForm.valid) {
      const governorateId = this.newPackageForm.value.governorateId;
      const cityId = this.newPackageForm.value.cityId;

      
      const cityExists = this.specialPackages.some(
        (pkg) => pkg.cityId == cityId
      );

      if (cityExists) {
        
        Swal.fire({
          icon: 'error',
          title: 'Error',
          text: 'There can be only one special package for each city.',
        });
        return;
      }

      const newPackage: IAddSpecialPackage = {
        governorateId,
        cityId,
        shippingPrice: this.newPackageForm.value.shippingPrice,
      };

      
      this.specialPackages.push(newPackage);

      
      const governorateName =
        this.governorates.find((g) => g.id == governorateId)?.name || 'Unknown';
      const cityName =
        this.cities.find((c) => c.id == cityId)?.name || 'Unknown';
      this.displaySpecialPackage.push({
        
        governorateName,
        cityName,
        shippingPrice: this.newPackageForm.value.shippingPrice,
        merchantName: this.addMerchantForm.value.storeName || '',
      });

      
      this.addMerchantForm.patchValue({
        specialPackages: this.specialPackages,
      });

      
      this.newPackageForm.reset();
      this.addSpecialPackage = false;

      
      Swal.fire({
        icon: 'success',
        title: 'Success',
        text: 'Special package added successfully!',
        showConfirmButton: false,
        timer: 1500,
      });
    } else {
      
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Please enter valid data for the special package.',
      });
    }
  }

  
  deleteSpecialPackage(index: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Do you really want to delete this special package?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, keep it',
    }).then((result) => {
      if (result.isConfirmed) {
        this.displaySpecialPackage.splice(index, 1);
        this.specialPackages.splice(index, 1);
        this.addMerchantForm.patchValue({
          specialPackages: this.specialPackages,
        });

        Swal.fire({
          icon: 'success',
          title: 'Deleted!',
          text: 'Special package has been deleted.',
          showConfirmButton: false,
          timer: 1500,
        });
      }
    });
  }

  
  onSubmit() {
    if (this.addMerchantForm.valid) {
      this.merchantService
        .Add('http://localhost:5241/api/Merchant', this.addMerchantForm.value)
        .subscribe(
          () => {
            Swal.fire({
              icon: 'success',
              title: 'Success',
              text: 'Merchant added successfully!',
              showConfirmButton: false,
              timer: 1500,
            }).then(() => {
              this.router.navigate(['/admin/merchant']);
            });
          },
          (error) => {
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
        );
    } else {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'Please enter valid data for the merchant.',
      });
      console.error('Form invalid:', this.addMerchantForm);
    }
  }
}