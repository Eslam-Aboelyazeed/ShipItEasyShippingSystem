import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { OrderReportComponent } from './shared-module/Components/order-report/order-report.component';
import { AccountService } from '../admin/Services/account.service';
import Swal from 'sweetalert2';
import { AuthService } from './shared-module/Services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, OrderReportComponent,RouterLink],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'ITI.FinalProject.Angular';
  
  currentUrl: string;

  role:string;

  lSub:any;

  constructor(private route: ActivatedRoute, private router:Router, private accountService:AccountService, private authService:AuthService) {
    this.currentUrl = '';
    this.role = '';
  }

  ngOnInit() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        this.currentUrl = this.router.url;
        console.log(this.currentUrl);
        this.role = this.authService.getRole() ?? '';
        console.log('role',this.role);
      }
    });
    
  }

  ngOnDestroy(): void {
    if (this.lSub != undefined) {
      this.lSub.unsubscribe();
    }
  }


  logout(){
    this.lSub = this.accountService.LogOut().subscribe({
      next:data => {
        this.authService.clear();
        this.router.navigate(['/login']);
      },
      error: error => {
        if (error.statusCode == 401) {
          Swal.fire({
            icon: "error",
            title: "Error",
            text: "You need to sign in first",
          })
        }
      }
    })
  }
}
