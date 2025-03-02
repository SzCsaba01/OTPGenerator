import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { IOTP } from "../models/OTP.model";
import { environment } from "../../../environments/environment.development";

@Injectable({
    providedIn: 'root'
})
export class OTPGeneratorService {
    private _url = 'OTPGenerator';

    constructor(private http: HttpClient) {}

    public generateOTP(): Observable<IOTP> {
        return this.http.get<IOTP>(`${environment.apiUrl}/${this._url}/GenerateOTP`);
    }

    public validateOTP(otp: IOTP): Observable<any> {
        return this.http.put(`${environment.apiUrl}/${this._url}/ValidateOTP`, otp);
    }
}