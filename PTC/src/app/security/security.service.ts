import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { AppUserAuth } from './app-user-auth';
import { AppUser } from './app-user';
import { tap } from 'rxjs/operators';
import { HttpHeaders, HttpClient } from '@angular/common/http';

const API_URL = 'http://localhost:5000/api/security/';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json'
  })
};

@Injectable({
  providedIn: 'root'
})
export class SecurityService {
  securityObject: AppUserAuth = new AppUserAuth();

  constructor(private http: HttpClient) {}

  resetSecurityObject(): void {
    this.securityObject.userName = '';
    this.securityObject.bearerToken = '';
    this.securityObject.isAuthenticated = false;
    this.securityObject.canAccessProducts = false;
    this.securityObject.canAddProduct = false;
    this.securityObject.canSaveProduct = false;
    this.securityObject.canAccessCategories = false;
    this.securityObject.canAddCategory = false;

    localStorage.removeItem('bearerToken');
  }

  logout(): void {
    this.resetSecurityObject();
  }

  login(entity: AppUser): Observable<AppUserAuth> {
    this.resetSecurityObject();

    return this.http
      .post<AppUserAuth>(API_URL + 'login', entity, httpOptions)
      .pipe(
        tap(resp => {
          // use object assign to update the current object
          // Note: Don't create a new AppUserAuth object
          //    because that destroys all refereences to object properties
          Object.assign(this.securityObject, resp);
          // Store into local storage
          localStorage.setItem('bearerToken', this.securityObject.bearerToken);
        })
      );
  }
}

/* Archive
  login(entity: AppUser): Observable<AppUserAuth> {
      this.resetSecurityObject();

      Object.assign(
        this.securityObject,

        LOGIN_MOCKS.find(
          user => user.userName.toLowerCase() === entity.userName.toLowerCase()
        )
      );

      if (this.securityObject.userName !== '') {
        localStorage.setItem('bearerToken', this.securityObject.bearerToken);
      }
      return of<AppUserAuth>(this.securityObject);
    }
  }
*/
