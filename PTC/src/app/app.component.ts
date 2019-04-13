import { Component } from '@angular/core';
import { SecurityService } from './security/security.service';
import { AppUserAuth } from './security/app-user-auth';

@Component({
  selector: 'ptc-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Paul\'s Training Company';
  securityObject: AppUserAuth = null;

  constructor(private securityService: SecurityService) {
    this.securityObject = securityService.securityObject;
  }

  logout(): void {
    this.securityService.logout();
  }
}
