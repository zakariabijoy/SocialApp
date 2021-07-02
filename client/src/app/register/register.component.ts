import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      userName: new FormControl('', Validators.required),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(4),
        Validators.maxLength(8),
      ]),
      confirmPassword: new FormControl('', Validators.required),
    });
  }

  register() {
    console.log(this.registerForm.value);
    // this.accountService.register(this.model).subscribe(
    //   (response) => {
    //     console.log(response);
    //     this.cancel();
    //     this.toastr.success('registrartion completed successfully');
    //   },
    //   (error) => {
    //     console.log(error);
    //     this.toastr.error(error.error);
    //   }
    // );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
