// src/plugins/ApiClient.ts
import axios from 'axios';
import type { AxiosRequestConfig, AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { useLocaleStore } from '@/stores/localeStore'; // localeStore theo Metronic folder stores
import { useNotificationStore } from '@/stores/notificationStore';

export interface BaseEntityModel {
  id?: string | null;                // Guid bên C# => string bên TS
  createdAt?: string;         // DateTime => string (ISO) hoặc Date tuỳ convention, thường là string
  updatedAt?: string | null; // DateTime? => string | null hoặc undefined
  isDeleted?: boolean;
  deletedAt?: string | null;
  createdBy?: string | null;
  updatedBy?: string | null;
  deletedBy?: string | null;
  status?: number; // Thêm trường status để quản lý trạng thái
}


export class ApiClient {
  protected api: AxiosInstance;
  private isRefreshing: boolean = false;
  private refreshSubscribers: Array<(token: string) => void> = [];

  constructor(baseURL: string) {
    this.api = axios.create({ baseURL });
    console.log(`API Client initialized with base URL: ${baseURL}`);
    this.api.interceptors.request.use(this.requestInterceptor.bind(this));
    this.api.interceptors.response.use(
      (response) => response.data, // Luôn trả về response.data
      this.responseInterceptor.bind(this)
    );
  }

  protected async requestInterceptor(config: InternalAxiosRequestConfig) {
    const localeStore = useLocaleStore();
    const lang = localeStore.currentLocale;

    const token = localStorage.getItem('accessToken');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    if (lang === 'vi') {
      config.headers['Accept-Language'] = 'vi-VN';
    } else {
      config.headers['Accept-Language'] = 'en-US';
    }
    config.headers.formName = sessionStorage.getItem('form_active') || '';
    return config;
  }

  private onRefreshed(token: string) {
    this.refreshSubscribers.forEach((callback) => callback(token));
    this.refreshSubscribers = [];
  }

  private addRefreshSubscriber(callback: (token: string) => void) {
    this.refreshSubscribers.push(callback);
  }

  protected async responseInterceptor(error: any) {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      console.warn('Unauthorized request, attempting to refresh token...');
      if (localStorage.getItem('refreshToken')) {
        if (!this.isRefreshing) {
          this.isRefreshing = true;

          try {
            const refreshToken = localStorage.getItem('refreshToken');
            const response = await axios.post(
              `${import.meta.env.VITE_APP_API_URL}/auth/refresh`,
              { refreshToken }
            );

            const newAccessToken = response.data.accessToken;
            const newRefreshToken = response.data.refreshToken;

            localStorage.setItem('accessToken', newAccessToken);
            localStorage.setItem('refreshToken', newRefreshToken);

            this.onRefreshed(newAccessToken);
            this.isRefreshing = false;
          } catch (refreshError) {
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            window.location.href = '/auth/login';
            return Promise.reject(refreshError);
          }
        }

        return new Promise((resolve) => {
          this.addRefreshSubscriber((token: string) => {
            if (originalRequest.headers) {
              originalRequest.headers.Authorization = `Bearer ${token}`;
            }
            originalRequest._retry = true;
            resolve(this.api(originalRequest));
          });
        });
      } else {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        console.warn('No refresh token available, redirecting to login...');
        window.location.href = '/auth/login';
      }
    }

    // Lấy store (nếu dùng Pinia)
    const notificationStore = useNotificationStore();
    // Lấy message lỗi từ backend hoặc mặc định
    const message =
      error?.response?.data?.errors ||
      error?.response?.data?.message || error?.message ||
      'toast.unexpectedError';
    //console.error('API Error: ', message, error);
    // Nếu là mảng, truyền nguyên mảng
    // Nếu là object { key, params }, truyền nguyên object
    // Nếu là chuỗi, truyền object { key: message }
    if (Array.isArray(message)) {
      message.forEach((msg) => {
        if (typeof msg === 'object' && msg.key) {
          notificationStore.showToast('error', msg);
        } else {
          notificationStore.showToast('error', { key: msg });
        }
      });
    } else if (typeof message === 'object' && message.key) {
      notificationStore.showToast('error', message);
    } else {
      notificationStore.showToast('error', { key: message });
    }
    // Trả lỗi cho phần gọi API nếu cần xử lý thêm
    return Promise.reject(error.response?.data || error);
  }

  public get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    console.log(`GET request to ${url} with config:`, config);

    return this.api.get<T>(url, config).then(res => {
      console.log(res);
      return res as T;
    });
  }

  public post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    console.log(`POST request to ${url} with data:`, data, 'and config:', config);
    return this.api.post<T>(url, data, config).then(res => {
      console.log(res);
      return res as T;
    });
  }

  public put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    console.log(`PUT request to ${url} with data:`, data, 'and config:', config);
    return this.api.put<T>(url, data, config).then(res => {
      console.log(res);
      return res as T;
    });
  }

  public patch<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    console.log(`PATCH request to ${url} with data:`, data, 'and config:', config);
    return this.api.patch<T>(url, data, config).then(res => {
      console.log(res);
      return res as T;
    });
  }

  public delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    console.log(`DELETE request to ${url} with config:`, config);
    return this.api.delete<T>(url, config).then(res => res as T);
  }
}

