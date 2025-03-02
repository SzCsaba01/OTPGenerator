import { Guid } from "guid-typescript";

export interface IOTP {
    id: Guid;
    oneTimePassword: string;
    expirationDate?: Date;
}