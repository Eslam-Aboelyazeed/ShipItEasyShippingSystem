export interface IAccount {
    id: string;
    name: string;
    email: string;
  }
  
  export interface IAccountView {
    id: string;
    name: string;
    email: string;
  }
  
  export interface LoginCredentials {
    emailOrUserName: string;
    password: string;
  }
  
  export interface LoginResponse {
    token: string;
    expireDate: string;
    Role: string;
    name: string;
  }
  