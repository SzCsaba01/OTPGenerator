import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { OTPGeneratorService } from '../../services/otp-generator.service';
import { OTP_LENGTH } from '../../utils/constants';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { IOTP } from '../../models/OTP.model';
import { ToastService } from '../../services/toast.service';
import { takeUntil } from 'rxjs';

@Component({
  selector: 'app-home',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent extends SelfUnsubscriberBase implements OnInit {
  otpForm!: FormGroup;
  otpGenerated = false;
  generatedOTP!: IOTP;
  countdownTimer: any;
  remainingTime: number = 0; 

  constructor(
    private otpGeneratorService: OTPGeneratorService,
    private toastService: ToastService,
    private formBuilder: FormBuilder,
  ) {
    super();
  }

  ngOnInit(): void {
    this.otpForm = this.formBuilder.group({
      otp: this.formBuilder.array(this.createOtpControls())
    });
  }

  createOtpControls(): any[] {
    return Array.from({ length: OTP_LENGTH }, () =>
      this.formBuilder.control('', [Validators.required, Validators.maxLength(1)])
    );
  }

  get otp() {
    return (this.otpForm.get('otp') as FormArray).controls;
  }

  onInput(event: any, index: number) {
    if (event.target.value.length === 1 && index < this.otp.length - 1) {
      (event.target.nextElementSibling as HTMLInputElement)?.focus();
    }
    if (this.isOtpComplete()) {
      this.validateOTP();
    }
  }

  onKeyDown(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;

    if (event.key === 'Backspace' && !this.otp[index].value && index > 0) {
      console.log(this.otp[index].value);
      (input.previousElementSibling as HTMLInputElement)?.focus();
    }
  }

  onClick(event: any) {
    const input = event.target as HTMLInputElement;
    input.selectionStart = input.selectionEnd = input.value.length;
  }

  generateOTP() {
    this.otpGenerated = true;

    this.toastService.clearMessages();
    this.otpForm.reset();

    this.otpGeneratorService.generateOTP()
    .pipe(takeUntil(this.ngUnsubscribe))
    .subscribe((otp: IOTP) => {
      this.generatedOTP = otp;
      const expirationTime = new Date(otp.expirationDate!).getTime();
      this.remainingTime = Math.floor((expirationTime - Date.now()) / 1000);

      this.toastService.showMessage(
        `Your One Time Password is: ${otp.oneTimePassword}. Please enter it to verify your identity.`,
        'success',
        this.remainingTime * 1000
      );

      this.startCountdown(expirationTime);
    });
  }

  private startCountdown(expirationTime: number) {
    if (this.countdownTimer) {
      clearInterval(this.countdownTimer);
    }

    this.countdownTimer = setInterval(() => {
      this.remainingTime = Math.floor((expirationTime - Date.now()) / 1000);

      if (this.remainingTime <= 0) {
        clearInterval(this.countdownTimer);
      }
    }, 1000);
  }

  isOtpComplete(): boolean {
    return this.otp.every(control => control.value.trim() !== '');
  }

  validateOTP() {
    const otpValue = this.otp.map(control => control.value).join('');
    const dto: IOTP = { id: this.generatedOTP.id, oneTimePassword: otpValue };

    this.otpGeneratorService.validateOTP(dto)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((result) => {
        if (!result) {
          return;
        }
        this.otpGenerated = false;
        this.toastService.clearMessages();
        this.toastService.showMessage(result.message, "success", 5000);
    });
  }
}
