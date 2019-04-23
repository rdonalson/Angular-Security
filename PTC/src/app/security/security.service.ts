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

  login(entity: AppUser): Observable<AppUserAuth> {
    this.resetSecurityObject();

    return this.http.post<AppUserAuth>(API_URL + 'login',
      entity, httpOptions).pipe(
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

  logout(): void {
    this.resetSecurityObject();
  }

  resetSecurityObject(): void {
    this.securityObject.userName = '';
    this.securityObject.bearerToken = '';
    this.securityObject.isAuthenticated = false;

    this.securityObject.claims = [];

    localStorage.removeItem('bearerToken');
  }

  /**
   *  This method can be called a couple of different ways
   *  *hasClaim="'claimType'" - Assumes claimValue is true
   *  *hasClaim="'claimType:value'" - Compares claimValue to value
   *  *hasClaim="['claimType:value','claimType2:value','claimType3']"
   */
  hasClaim(claimType: any, claimValue?: any): boolean {
    // tslint:disable-next-line:no-inferrable-types
    let ret: boolean = false;

    // See if an array of values was passed in.
    if (typeof claimType === 'string') {
      ret = this.isClaimValid(claimType, claimValue);
    } else {
      const claims: string[] = claimType;
      if (claims) {
        for (const claim of claims) {
          ret = this.isClaimValid(claim);
          if (ret) {
            break;
          }
        }
      }
    }
    return ret;
  }

  private isClaimValid(claimType: string, claimValue?: string): boolean {
    // tslint:disable-next-line:no-inferrable-types
    let ret: boolean = false;
    let auth: AppUserAuth = null;

    // Retrieve security object
    auth = this.securityObject;
    if (auth) {
      // See if the claim type has a value
      // *hasClaim="'claimType:value'"
      if (claimType.indexOf(':') >= 0) {
        const words: string[] = claimType.split(':');
        claimType = words[0].toLowerCase();
        claimValue = words[1].toLowerCase();
      } else {
        claimType = claimType.toLowerCase();
        // Either get the claim value, or assume 'true'
        claimValue = claimValue ? claimValue.toLowerCase() : 'true';
      }

      // Attempt to find the claim
      ret =
        auth.claims.find(
          c =>
            // tslint:disable-next-line: triple-equals
            c.claimType.toLowerCase() === claimType &&
            // tslint:disable-next-line: triple-equals
            c.claimValue.toLowerCase() === claimValue
        ) != null;
    }
    return ret;
  }

}
