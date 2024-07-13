import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ISettings } from '../../../admin/DTOs/DisplayDTOs/ISettings';
import { GenericService } from '../../../admin/Services/generic.service';
import { IAddSettings } from '../../../admin/DTOs/InsertDTOs/IAddSettings';
import { IUpdateSettings } from '../../../admin/DTOs/UpdateDTOs/IUpdateSettings'
import Swal from 'sweetalert2';
@Component({
  selector: 'app-setting-list',
  standalone: true,
  imports: [CommonModule,FormsModule,RouterModule],
  templateUrl: './setting-list.component.html',
  styleUrl: './setting-list.component.css'
})
export class SettingListComponent implements OnInit, OnDestroy {
  baseUrl:string="http://localhost:5241/api/";
settingsList:ISettings[]=[];

sSub:any;
constructor(
private settingService:GenericService<ISettings,IAddSettings,IUpdateSettings>,
private router:Router
) { }

  ngOnDestroy(): void {
    if (this.sSub != undefined) {
      this.sSub.unsubscribe();
    }
  }

  ngOnInit(): void {
    this.loadSettings();
  }
  loadSettings(): void {
    this.sSub = this.settingService.GetAll(this.baseUrl+"Settings").subscribe(
      (settings) => {
        console.log(settings);
        this.settingsList = settings;
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
  }

}
