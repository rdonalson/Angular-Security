import { Component, OnInit } from '@angular/core';
import { AppUser } from './app-user';
import { AppUserAuth } from './app-user-auth';
import { SecurityService } from './security.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'ptc-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  user: AppUser = new AppUser();
  securityObject: AppUserAuth = null;
  returnUrl: string;
  constructor(
    private securityService: SecurityService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
  }

  login(): void {
    this.securityService.login(this.user).subscribe(resp => {
      this.securityObject = resp;
      if (this.returnUrl) {
        this.router.navigateByUrl(this.returnUrl);
      }
    });
  }
}
