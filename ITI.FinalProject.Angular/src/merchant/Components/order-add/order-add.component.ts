import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  FormArray,
  ReactiveFormsModule,
} from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { OrderService } from '../../Services/order.service';
import { ProductService } from '../../Services/product.service';
import { CityService } from '../../Services/city.service';
import { GovernorateService } from '../../Services/governorate.service';
import { BranchService } from '../../Services/branch.service';
import { IAddOrder } from '../../DTOs/Insert DTOs/IAddOrder';
import { IAddProduct } from '../../DTOs/Insert DTOs/IAddProduct';
import { OrderStatus } from '../../Enums/OrderStatus';
import { OrderTypes } from '../../Enums/OrderTypes';
import { PaymentTypes } from '../../Enums/PaymentTypes';
import { ShippingTypes } from '../../Enums/ShippingTypes';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { AuthService } from '../../../app/shared-module/Services/auth.service';

@Component({
  selector: 'app-order-add',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './order-add.component.html',
  styleUrls: ['./order-add.component.css'],
})
export class OrderAddComponent implements OnInit, OnDestroy {
  orderForm: FormGroup | undefined;
  cities: any[] = [];
  filteredCities: any[] = [];
  governorates: any[] = [];
  branches: any[] = [];
  filteredBranches: any[] = [];

  addingProduct: boolean = false;
  merchantId: string | null = null;
  subscriptions: Subscription = new Subscription();

  constructor(
    private formBuilder: FormBuilder,
    private orderService: OrderService,
    private productService: ProductService,
    private cityService: CityService,
    private governorateService: GovernorateService,
    private branchService: BranchService,
    private router: Router,
    private authService:AuthService
  ) {}

  ngOnInit() {
    
    this.orderForm = this.formBuilder.group({
      clientName: ['', Validators.required],
      phone: ['', Validators.required],
      phone2: [''],
      email: ['', [Validators.email]],
      villageAndStreet: ['', Validators.required],
      shippingToVillage: [false],
      governorateId: [0, Validators.required],
      cityId: [0, Validators.required],
      shippingId: [0, Validators.required],
      branchId: [0, Validators.required],
      status: [OrderStatus.New, Validators.required],
      type: [0, Validators.required],
      paymentType: [0, Validators.required],
      shippingType: [0, Validators.required],
      products: this.formBuilder.array([]),
    });

    this.loadDropdownData();

    const governorateSub = this.orderForm
      .get('governorateId')
      ?.valueChanges.subscribe({
      next:  (governorateId) => {
        this.filterCities(governorateId);
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

    const citySub = this.orderForm
      .get('cityId')
      ?.valueChanges.subscribe({
      next:  (cityId) => {
        this.filterBranches(cityId);
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

    this.subscriptions.add(governorateSub!);
    this.subscriptions.add(citySub!);
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  loadDropdownData() {
    const url = 'http://localhost:5241/api/';
    const citySub = this.cityService
      .GetOptions(url + 'cityOptions')
      .subscribe({
      next:  (data) => {
        
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
      next:  (data) => {
        
        
        
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
      next:  (data) => {
        
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
    this.orderForm?.get('cityId')?.setValue('');
    this.filteredBranches = [];
    this.orderForm?.get('branchId')?.setValue('');
  }

  filterBranches(cityId: number) {
    this.filteredBranches = this.branches.filter(
      (branch) => branch.dependentId == cityId
    );
    this.orderForm?.get('branchId')?.setValue('');
  }

  getEnumKeyValue(enumObj: any): { key: number; value: string }[] {
    return Object.keys(enumObj)
      .filter((key) => !isNaN(Number(key)))
      .map((key) => ({ key: Number(key), value: enumObj[key] }));
  }

  get orderTypes() {
    return this.getEnumKeyValue(OrderTypes);
  }

  get paymentTypes() {
    return this.getEnumKeyValue(PaymentTypes);
  }

  get orderStatuses() {
    return this.getEnumKeyValue(OrderStatus);
  }

  get shippingTypes() {
    return this.getEnumKeyValue(ShippingTypes);
  }

  get products(): FormArray {
    return this.orderForm?.get('products') as FormArray;
  }

  toggleAddProduct() {
    if (this.addingProduct) {
      if (this.orderForm && this.orderForm.valid) {
        this.saveProduct();
        Swal.fire('Success', 'Product saved successfully!', 'success');
      } else {
        console.log(this.orderForm?.value);
        Swal.fire('Error', 'Please enter valid product data.', 'error');
      }
    } else {
      this.addProductRow();
    }
  }

  addProductRow() {
    const productGroup = this.formBuilder.group({
      name: ['', Validators.required],
      weight: [0, Validators.required],
      quantity: [0, Validators.required],
      price: [0, Validators.required],
      statusNote: [''],
      ProductStatus: [OrderStatus.Pending, Validators.required],
    });

    this.products.push(productGroup);
    this.addingProduct = true; 
  }

  saveProduct() {
    
    
    
    
    
    
    
    
    
    
    
    
    
    const productData = this.products.at(this.products.length - 1).value;
    const productFormGroup = this.formBuilder.group(productData);
    
    this.addingProduct = false;
  }

  removeProduct(index: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Do you want to remove this product?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, keep it',
    }).then((result) => {
      if (result.isConfirmed) {
        this.products.removeAt(index);
        this.addingProduct = false; 
        Swal.fire('Deleted!', 'Product has been removed.', 'success');
      }
    });
  }

  onSubmit() {
    this.merchantId = this.authService.getMerchantId();
    console.log(this.orderForm?.value);
    if (this.orderForm?.invalid) {
      Swal.fire('Error', 'Please enter valid order data.', 'error');
    } else if (this.orderForm && this.orderForm.valid) {
      const orderData: IAddOrder = {
        clientName: this.orderForm.controls['clientName'].value,
        phone: this.orderForm.controls['phone'].value,
        phone2: this.orderForm.controls['phone2'].value,
        email: this.orderForm.controls['email'].value,
        villageAndStreet: this.orderForm.controls['villageAndStreet'].value,
        shippingToVillage: this.orderForm.controls['shippingToVillage'].value,
        governorateId: Number(this.orderForm.controls['governorateId'].value),
        cityId: Number(this.orderForm.controls['cityId'].value),
        shippingId: Number(this.orderForm.controls['shippingId'].value),
        branchId: Number(this.orderForm.controls['branchId'].value),
        status: Number(this.orderForm.controls['status'].value),
        type: Number(this.orderForm.controls['type'].value),
        paymentType: Number(this.orderForm.controls['paymentType'].value),
        shippingType: Number(this.orderForm.controls['shippingType'].value),
        products: this.products.value.map((product: any) => ({
          ...product,
          weight: +product.weight,
          quantity: +product.quantity,
          price: +product.price,
        })),
        merchantId: this.merchantId || '',
      };
      console.log(orderData);
      const url = 'http://localhost:5241/api/Orders';
      let aoSub = this.orderService.Add(url, orderData).subscribe({
        next:(response) => {
          console.log('Order and products added successfully', response);
          Swal.fire('Success', 'Order submitted successfully!', 'success');
          this.router.navigate(['/orderShow']);
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

      this.subscriptions.add(aoSub);
    }
  }
}
