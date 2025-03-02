export interface IToastMessage {
    id: number;
    message: string;
    type: 'success' | 'error' | 'clear';
    expiration: number;
  }