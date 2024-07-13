import { IBranch } from './../../DTOs/DisplayDTOs/IBranch';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { GenericService } from '../../Services/generic.service';
import { IBranchInsert } from '../../DTOs/InsertDTOs/IBranchInsert';
import { IBranchUpdate } from '../../DTOs/UpdateDTOs/IBranchUpdate';
import { ICity } from '../../DTOs/DisplayDTOs/ICity';
import { ICityInsert } from '../../DTOs/InsertDTOs/ICityInsert';
import { ICityUpdate } from '../../DTOs/UpdateDTOs/ICityUpdate';
import { BranchService } from '../../Services/branch.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-branch-from',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './branch-from.component.html',
  styleUrl: './branch-from.component.css',
})
export class BranchFromComponent implements OnInit, OnDestroy {
  baseURL: string = 'http://localhost:5241/api/';
  branchId: any = 0;
  branch: any = {
    id: 0,
    name: '',
    status: 0,
    addingDate: new Date(),
    cityId: 0,
  };
  cities: any[] = [];

  zSub: any;
  cSub: any;
  bSub: any;
  brSub: any;

  constructor(
    public route: ActivatedRoute,
    public routing: Router,

    public branchServ: GenericService<IBranch, IBranchInsert, IBranchUpdate>,
    public cityServ: GenericService<ICity, ICityInsert, ICityUpdate>
  ) {}

  ngOnDestroy(): void {
    if (this.zSub != undefined) {
      this.zSub.unsubscribe();
    }
    if (this.cSub != undefined) {
      this.cSub.unsubscribe();
    }
    if (this.bSub != undefined) {
      this.bSub.unsubscribe();
    }
    if (this.brSub != undefined) {
      this.brSub.unsubscribe();
    }
  }

  branchForm = new FormGroup({
    id: new FormControl(0),
    name: new FormControl('', [Validators.required, Validators.minLength(2)]),
    status: new FormControl(1, [Validators.required]),
    addingDate: new FormControl(new Date()),
    cityId: new FormControl(0, [Validators.required, Validators.min(1)]),
  });

  get getId() {
    return this.branchForm.controls['id'];
  }
  get getName() {
    return this.branchForm.controls['name'];
  }
  get getStatus() {
    return this.branchForm.controls['status'];
  }
  get getAddingDate() {
    return this.branchForm.controls['addingDate'];
  }
  get getCity() {
    return this.branchForm.controls['cityId'];
  }

  ngOnInit(): void {
    this.cSub = this.cityServ
      .GetOptions(this.baseURL + 'cityOptions')
      .subscribe({
        next: (value) => {
          console.log(value);
          this.cities = value;
        },
        error: (error) => {
          if (error.status == 401) {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: 'Unauthorized',
            });
          } else if (error.error.message) {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: error.error.message,
            });
          } else {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: 'Something went wrong, please try again later',
            });
          }
        },
      });

    this.zSub = this.route.params.subscribe({
      next: (params) => {
        this.branchId = params['id'];

        if (this.branchId) {
          this.bSub = this.branchServ
            .GetById(this.baseURL + 'branches/' + this.branchId)
            .subscribe({
              next: (value) => {
                this.branch = value;

                this.getId.setValue(this.branch.id);
                this.getName.setValue(this.branch?.name);
                this.getCity.setValue(this.branch.cityId);
                this.getStatus.setValue(this.branch.status);
                this.getAddingDate.setValue(new Date());
              },
              error: (error) => {
                if (error.status == 401) {
                  Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Unauthorized',
                  });
                } else if (error.error.message) {
                  Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: error.error.message,
                  });
                } else {
                  Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Something went wrong, please try again later',
                  });
                }
              },
            });
        }
      },
    });
  }

  onSubmit() {
    this.branchForm.controls['cityId'].setValue(Number(this.getCity.value));
    this.branchForm.controls['status'].setValue(Number(this.getStatus.value));
    this.branchForm.controls['id'].setValue(Number(this.branchId));

    this.branch = this.branchForm.value;

    if (this.branchId) {
      this.brSub = this.branchServ
        .Edit(this.baseURL + 'branches/' + this.branchId, this.branch)
        .subscribe({
          next: (value) => {
            this.routing.navigate(['/admin/branch']);
          },
          error: (error) => {
            if (error.status == 401) {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Unauthorized',
              });
            } else if (error.error.message) {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error.error.message,
              });
            } else {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Something went wrong, please try again later',
              });
            }
          },
        });
    } else {
      let newBranch: any = {
        name: this.getName.value,
        status: this.getStatus.value,
        cityId: this.getCity.value,
      };
      this.brSub = this.branchServ
        .Add(this.baseURL + 'branches', newBranch)
        .subscribe({
          next: (value) => {
            this.routing.navigate(['/admin/branch']);
          },
          error: (error) => {
            if (error.status == 401) {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Unauthorized',
              });
            } else if (error.error.message) {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error.error.message,
              });
            } else {
              Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Something went wrong, please try again later',
              });
            }
          },
        });
    }
  }
}
