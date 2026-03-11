export interface Result<T = any> {
  succeeded: boolean;
  errors: string;
  data?: T;
  statusCode?: number;
  message?: string;
}