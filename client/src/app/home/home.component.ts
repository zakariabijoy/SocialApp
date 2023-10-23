import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode =false;

  constructor(private router: Router) { }

  ngOnInit(): void {
   
  }

  registerToggle(){
    this.registerMode = !this.registerMode
  }

  cancelRegisterModel(event: boolean){
    this.registerMode = event;
  }

  learnMore(){
    this.router.navigateByUrl('/members');
  }

}
