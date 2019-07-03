import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  constructor(private http : HttpClient) { }
  // Url Access to Web Api
  private baseUrlLogin : string = "/api/account/login";
  // Information that i need
  private loginStatus :Observable<boolean> ;
  private userName : string;
  private userRole :string;
// We are using the RxJS

  login(){

    //validate User Credentials

    // Call login API

    //If user Authenticated then :
    

  }

}
