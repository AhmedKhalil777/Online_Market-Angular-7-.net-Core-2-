import { Component, OnInit } from '@angular/core';
import { AccountService } from '../service/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})


export class LoginComponent implements OnInit {
  private loginStatusGet : boolean = true;
  constructor(private acct :AccountService) { }

  ngOnInit() {
  
  }
  

}

